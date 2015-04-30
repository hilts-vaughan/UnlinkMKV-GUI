using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnlinkMKV_GUI
{
    public partial class FormApplication : Form
    {
        // This is a task run by UnlinkMKV with the intention of performing batch jobs
        private Thread _taskThread;

        public FormApplication()
        {
            InitializeComponent();

            textOutput.Text = Properties.Settings.Default["output"].ToString();
            textInput.Text = Properties.Settings.Default["input"].ToString();


            // Create the default path if needed
            if (string.IsNullOrEmpty(textOutput.Text))
            {
                string defaultPath = Path.Combine(Environment.CurrentDirectory, "output");
                Directory.CreateDirectory(defaultPath);
                textOutput.Text = defaultPath;
            }

            // Create our options
            CreateOptions();
        }

        private void CreateOptions()
        {
            // Setup a list of tuples for the options that matter

            var optionList = new List<Tuple<string, string>>();

            bool foundMpeg = true;

            try
            {
                PathUtility.FindExePath("ffmpeg.exe");
            }
            catch (FileNotFoundException exception)
            {
                foundMpeg = false;
            }

            if (foundMpeg)
            {
                optionList.Add(Tuple.Create("Fix audio", "--fix-audio"));
                optionList.Add(Tuple.Create("Fix video", "--fix-video"));
            }

            optionList.Add(Tuple.Create("Fix subtitles", "--fix-subtitles"));
            optionList.Add(Tuple.Create("Ignore default flag", "--ignore-default-flag"));
            optionList.Add(Tuple.Create("Ignore missing segments", "--ignore-missing-segments"));

            optionList.Add(Tuple.Create("Verbose output", "--ll TRACE"));




            foreach (var option in optionList)
            {
                var checkBox = new CheckBox();
                checkBox.Text = option.Item1;
                checkBox.Tag = option.Item2;
                checkBox.AutoSize = true;
                flowLayoutPanel1.Controls.Add(checkBox);
            }

        }

        private void FormApplication_Load(object sender, EventArgs e)
        {

            // Open the validator
            var validator = new FormValidator();
            validator.ShowDialog();
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            BrowseForFile(textInput);
        }

        private void buttonOutput_Click(object sender, EventArgs e)
        {
            BrowseForFile(textOutput);
        }

        private void BrowseForFile(TextBox textToUpdate)
        {

            var browse = new FolderBrowserDialog();
            var result = browse.ShowDialog();

            if (result == DialogResult.OK)
            {
                textToUpdate.Text = browse.SelectedPath;
            }

        }

        private string GetCommandLineArguments()
        {
            string buffer = "";

            foreach (var checkbox in flowLayoutPanel1.Controls.OfType<CheckBox>())
            {
                if (checkbox.Checked)
                {
                    buffer += checkbox.Tag + " ";
                }
            }
            return buffer;
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            // Abort the thread immediately
            _taskThread.Abort();

            // Force a cleanup of all stray processes that might be around
            var processToKill = new List<String>() { "mkvmerge", "mkvinfo", "mkvextract", "ffmpeg" };
            processToKill.ForEach(x => Process.GetProcessesByName(x).ToList().ForEach(process => process.Kill()));

            if (_currProcess != null)
                _currProcess.Kill();

            buttonExecute.Enabled = true;
            buttonAbort.Enabled = false;

        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            buttonExecute.Enabled = false;
            buttonAbort.Enabled = true;

            if (VerifyReady())
            {
                _taskThread = new Thread(PerformJob);
                _taskThread.Start();
            }
        }

        private Process _currProcess;

        private bool VerifyReady()
        {
            bool ready = true;


            if (!Directory.Exists(textOutput.Text) || !Directory.Exists(textInput.Text))
            {
                ready = false;
                MessageBox.Show("Make sure to select some input and output paths that are valid before unlinking.");
            }

            return ready;
        }

        private string CleanEscape(string inputString)
        {
            return Regex.Replace(inputString, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
        }

        private void PerformJob()
        {
            textLog.Clear();

            // Create directory ahead of time
            Directory.CreateDirectory(textOutput.Text);

            var files = Directory.GetFiles(textInput.Text);

            var perlParams = GetCommandLineArguments();
            var perlPath = PathUtility.FindExePath("perl.exe");
            var outDirectory = "--outdir \"" + textOutput.Text + "\"";
            var perlScript = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "winport.pl");


            // For each file, we need to run the Perl script once. It doesn't support batch mode
            foreach (var file in files)
            {
                // Generate a Perl job
                var quotedFile = "\"" + file + "\"";
                var argument = "\"" + perlScript + "\" " + perlParams + " " + outDirectory + " " + quotedFile;

                // PathUtility.ExceptionalPath
                var startInfo = new ProcessStartInfo
                {
                    FileName = perlPath,
                    Arguments = argument,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Add the path if required to the Perl executing path
                if (!string.IsNullOrEmpty(PathUtility.ExceptionalPath))
                {
                    startInfo.EnvironmentVariables["PATH"] = PathUtility.ExceptionalPath;
                }

                var perlJob = new Process()
                {
                    StartInfo = startInfo
                };


                perlJob.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

                perlJob.Start();

                _currProcess = perlJob;

                while (!perlJob.StandardOutput.EndOfStream)
                {
                    string line = perlJob.StandardOutput.ReadLine();
                    ExecuteSecure(() => textLog.AppendText(CleanEscape(line) + Environment.NewLine));                    
                }

                _currProcess.WaitForExit();
   
                _currProcess = null;
            }


            // End the job
            ExecuteSecure(() => MessageBox.Show("The unlinking is complete!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information));
            
            buttonExecute.Enabled = true;
            buttonAbort.Enabled = false;
        }

        private void ExecuteSecure(Action a)
        {
            if (InvokeRequired)
                BeginInvoke(a);
            else
                a();
        }

        private void FormApplication_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_taskThread != null && _taskThread.IsAlive)
            {
                e.Cancel = true;
                MessageBox.Show(
                    "The conversion process is not yet complete. Please abort the current job first before closing.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Save settings
            Properties.Settings.Default["input"] = textInput.Text;
            Properties.Settings.Default["output"] = textOutput.Text;

            // Save the settings to the disk
            Properties.Settings.Default.Save();

        }

    }
}
