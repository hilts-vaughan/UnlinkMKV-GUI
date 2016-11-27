using System.Linq;
using System.Windows.Forms;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnlinkMKV_GUI.legacy;
using UnlinkMKV_GUI.merge;
using UnlinkMKV_GUI.ui;

namespace UnlinkMKV_GUI {
    public partial class FormApplication : Form {
        // This is a task run by UnlinkMKV with the intention of performing batch jobs
        private Thread _taskThread;

        public FormApplication() {
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

        private void CreateOptions() {
            // Setup a list of tuples for the options that matter

            var optionList = new List<Tuple<string, string>>();

            bool foundMpeg = true;

            try {
                PathUtility.FindExePath("ffmpeg.exe");
            }
            catch (FileNotFoundException exception) {
                foundMpeg = false;
            }

            if (foundMpeg)
            {
                optionList.Add(Tuple.Create("Fix audio", "--fixaudio"));
                optionList.Add(Tuple.Create("Fix video", "--fixvideo"));
            }

            optionList.Add(Tuple.Create("Fix subtitles", "--fixsubtitles"));
            optionList.Add(Tuple.Create("Ignore default flag", "--ignoredefaultflag"));
            optionList.Add(Tuple.Create("Ignore missing segments", "--ignoremissingsegments"));

            optionList.Add(Tuple.Create("Verbose output", "--ll TRACE"));
            optionList.Add(Tuple.Create("Native Backend", "--native"));

            foreach (var option in optionList) {
                var checkBox = new CheckBox();
                checkBox.Text = option.Item1;
                checkBox.Tag = option.Item2;
                checkBox.AutoSize = true;
                flowLayoutPanel1.Controls.Add(checkBox);
            }

        }

        private void FormApplication_Load(object sender, EventArgs e) {

            // Open the validator
            var validator = new FormValidator();
            validator.ShowDialog();
        }

        private void buttonInput_Click(object sender, EventArgs e) {
            BrowseForFile(textInput);
        }

        private void buttonOutput_Click(object sender, EventArgs e) {
            BrowseForFile(textOutput);
        }

        private void BrowseForFile(TextBox textToUpdate) {

            var browse = new FolderBrowserDialog();
            var result = browse.ShowDialog();

            if (result == DialogResult.OK)
            {
                textToUpdate.Text = browse.SelectedPath;
            }

        }

        private string GetCommandLineArguments() {
            string buffer = "";

            foreach (var checkbox in flowLayoutPanel1.Controls.OfType<CheckBox>()) {
                if (checkbox.Checked)
                {
                    buffer += checkbox.Tag + " ";
                }
            }
            return buffer;
        }

        private bool ShouldUseNativeBackend()
        {
            return GetCommandLineArguments().IndexOf("native", StringComparison.Ordinal) > -1;
        }

        private void buttonAbort_Click(object sender, EventArgs e) {
            // Abort the thread immediately
            _taskThread.Abort();

            // Force a cleanup of all stray processes that might be around
            var processToKill = GetUtilityPrograms();
            processToKill.ForEach(x => Process.GetProcessesByName(x).ToList().ForEach(process => process.Kill()));

            _currProcess?.Kill();

            buttonExecute.Enabled = true;
            buttonAbort.Enabled = false;

        }

        private List<String> GetUtilityPrograms() {
            return new List<String>() { "mkvmerge", "mkvinfo", "mkvextract", "ffmpeg", "mediainfo" };
        }

        private void buttonExecute_Click(object sender, EventArgs e) {
            buttonExecute.Enabled = false;
            buttonAbort.Enabled = true;

            if (VerifyReady())
            {
                _taskThread = new Thread(PerformJob);
                _taskThread.Start();
            }
        }

        private Process _currProcess;

        private bool VerifyReady() {
            bool ready = true;


            if (!Directory.Exists(textOutput.Text) || !Directory.Exists(textInput.Text))
            {
                ready = false;
                MessageBox.Show("Make sure to select some input and output paths that are valid before unlinking.");
            }

            return ready;
        }


        private async void PerformJob()
        {
            var isNative = ShouldUseNativeBackend();

            // Clear log
            textLog.Clear();

            // Create directory ahead of time
            Directory.CreateDirectory(textOutput.Text);

            IMkvJobMerger mergeStrategy;
            var logger = new TextBoxControlWriter(this, textLog);

            if (isNative)
            {
                mergeStrategy = new MergeJob(new MergeOptions());
                Console.SetOut(logger);
            }
            else
            {
                mergeStrategy = new PerlJob(GetCommandLineArguments());
            }

            var sourcePath = textInput.Text;
            var files = Directory.GetFiles(sourcePath);

            foreach (var file in files)
            {
                var result = mergeStrategy.ExecuteJob(logger, file, textOutput.Text);
            }

            // End the job
            ExecuteSecure(() => MessageBox.Show("The unlinking is complete!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information));

            buttonExecute.Enabled = true;
            buttonAbort.Enabled = false;
        }

        private void ExecuteSecure(Action a) {
            if (InvokeRequired)
                BeginInvoke(a);
            else
                a();
        }

        private void FormApplication_FormClosing(object sender, FormClosingEventArgs e) {
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

    /// <summary>
    /// A basic config file that is used to read and write to the config file provided by UnlinkMKV
    /// </summary>
    public class Config {
        private Dictionary<String, String> _values = new Dictionary<String, String>();

        public Config(string fileName) {
            using (TextReader reader = File.OpenText(fileName)) {
                string line = null;
                while ((line = reader.ReadLine()) != null) {
                    if (line == null) {
                        break;
                    }
                    // Now, chomp each line down
                    string[] chomped = line.Trim().Split("=".ToCharArray());
                    string key = chomped[0].Trim();
                    string value = chomped[1].Trim();
                    _values.Add(key, value);
                }
            }

            // The temporary path should just be set to what's normal for you
            _values["tmpdir"] = Path.Combine(Path.GetTempPath(), "UnlinkMKV");

            //
            //            ffmpeg = $basedir/ffmpeg/bin/ffmpeg
            //            mkvext = $basedir/mkvtoolnix/mkvextract
            //            mkvinfo = $basedir/mkvtoolnix/mkvinfo
            //            mkvmerge = $basedir/mkvtoolnix/mkvmerge

        }

        /// <summary>
        /// This function should be invoked to set the paths properly when the config is being constructed.1
        /// </summary>
        /// <param name="ffmpeg"></param>
        /// <param name="mkvext"></param>
        /// <param name="mkvinfo"></param>
        /// <param name="mkvmerge"></param>
        public void SetRequiredPaths(string ffmpeg, string mkvext, string mkvinfo, string mkvmerge) {
            _values["ffmpeg"] = ffmpeg;
            _values["mkvext"] = mkvext;
            _values["mkvinfo"] = mkvinfo;
            _values["mkvmerge"] = mkvmerge;
        }

        public string OutputPath {
            get {
                return _values["out_dir"];
            }
            set {
                _values["out_dir"] = value;
            }
        }

        // TODO; Add the rest of the flags in here when it's able

        public void Persist(string fileName) {
            // Delete the file prior (mono bug?)
            File.Delete(fileName);

            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            using (var writer = new StreamWriter(fs)) {
                foreach (var entry in _values) {
                    string lineEntry = string.Format("{0} = {1}", entry.Key, entry.Value);
                    writer.WriteLine(lineEntry);
                }
            }

            // stream writer is now closed at this point in time; all set!
        }

    }

}
