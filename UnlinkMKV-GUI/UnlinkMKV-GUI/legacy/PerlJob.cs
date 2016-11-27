using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnlinkMKV_GUI.merge;

namespace UnlinkMKV_GUI.legacy
{
    public class PerlJob : IMkvJobMerger
    {
        private readonly string _options;

        public PerlJob(string options)
        {
            _options = options;
        }

        public async Task<MergeResult> ExecuteJob(TextWriter log, string source, string destination)
        {
            var configPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "unlinkmkv.ini");
            var config = new Config(configPath);

            var merge = PathUtility.FindExePath("mkvmerge.exe");
            var info = PathUtility.FindExePath("mkvinfo.exe");
            var extract = PathUtility.FindExePath("mkvextract.exe");
            var ffmpeg = "";

            try
            {
                ffmpeg = PathUtility.FindExePath("ffmpeg.exe");
            }
            catch(Exception e)
            {
                // Do nothing
            }

            config.OutputPath = destination;
            config.SetRequiredPaths(ffmpeg, extract, info, merge);
            config.Persist(configPath);

            var perlParams = _options;
            var perlPath = PathUtility.FindExePath("perl.exe");
            var outDirectory = "--outdir \"" + destination+ "\"";
            var perlScript = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "winport.pl");

            var file = source;


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

                string x = perlPath + " " + argument;
                log.WriteLine(x);

                // Add the path if required to the Perl executing path
                if (!string.IsNullOrEmpty(PathUtility.ExceptionalPath))
                {
                    startInfo.EnvironmentVariables["PATH"] = PathUtility.ExceptionalPath;
                }

                var perlJob = new Process()
                {
                    StartInfo = startInfo
                };


                perlJob.ErrorDataReceived += (s, e) => log.WriteLine(e.Data);

                perlJob.Start();

                while (!perlJob.StandardOutput.EndOfStream) {
                    var line = perlJob.StandardOutput.ReadLine();
                    log.WriteLine(line);
                }

                perlJob.WaitForExit();

            return MergeResult.OK;
        }
    }
}