using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnlinkMKV_GUI.data;
using UnlinkMKV_GUI.merge.extract;
using UnlinkMKV_GUI.merge.info;

namespace UnlinkMKV_GUI.merge
{

    public class MergeJob : IMkvJobMerger
    {
        private readonly MergeOptions _options;
        private string _filename;
        private string _destination;
        private string _baseFilename;
        private List<string> _timecodes = new List<string>();
        private List<string> _segMapping = new List<string>();

        public MergeJob(MergeOptions options)
        {
            _options = options;
        }

        public async Task<MergeResult> ExecuteJob(TextWriter logger, string source, string destination)
        {
            this._filename = source;
            this._destination = destination;
            this._timecodes.Clear();
            this._segMapping.Clear();

            return PerformMerge();
        }

        public MergeResult PerformMerge()
        {
            Console.WriteLine("Starting the processing of the file: {0}", _filename);
            _baseFilename = Path.GetFileName(_filename);

            var loader = new MkvToolNixMkvInfoLoaderStrategy();
            var info = loader.FetchMkvInfo(_filename);

            if (info.IsFileLinked())
            {
                var workingDir = PathUtil.GetTemporaryDirectory();
                Environment.CurrentDirectory = workingDir;

                try
                {
                    Console.WriteLine("File was deteremined to be linked so will begin process...");
                    Console.WriteLine($"Your working directory will be: {workingDir}");

                    // Step 1: Capture the segments that are the same in the directory
                    var segments = SegmentUtility.GetMergePartsForFilename(_filename, info);

                    // Step 2: Extract all attachments from the current segments ++ extract out the main attachments, too
                    // segments.ForEach(ExtractAttachments);

                    var selector = new SegmentTimecodeSelector();
                    var dto = selector.GetTimecodeAndSegments(info, segments, false);
                    this._segMapping = dto.SegmentMap;
                    this._timecodes = dto.Timecodes;

                    // Step 4: Extract the splits
                    ExtractSplits(_filename);

                    // Step 5: Let's rebuild the file... the timecodes can tell us quite a bit but let's use the fully
                    // built "order"
                    RebuildFile();

                    File.Move(Path.Combine(workingDir, _baseFilename), Path.Combine(_destination, _baseFilename));
                    Console.WriteLine("Job complete!");
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    // Clean up after ones self
                    Directory.Delete(workingDir, true);
                }
            }
            else
            {
                Console.WriteLine("File was determined to not be linked. Ignoring.");
                return MergeResult.NothingToMerge;
            }

            return MergeResult.OK;
        }

        private void RebuildFile()
        {

            var proc = new ProcessStartInfo("mkvmerge");

            var y = new List<string>();
            foreach (var s in _segMapping)
            {
                y.Add(string.Format("\"{0}\"", s));
            }

            // Removing chapters options helps with the following issues:
            //  1. Ordered flags will get left behind sometimes, which is a nuissance
            //  2. Artifacted chapters are rarely useful when played back in something like Plex
            var parts = string.Join(" --no-chapters +", y);

            proc.Arguments = $"--no-chapters -o {_baseFilename} {parts}";
            var process = new Process();
            process.StartInfo = proc;
            process.Start();
            process.WaitForExit();
        }

        private int TimecodeComparator(ChapterAtom x, ChapterAtom y)
        {
            var firstStart = TimeCodeUtil.TimeCodeToTimespan(x.ChapterTimecodeStart);
            var secondStart = TimeCodeUtil.TimeCodeToTimespan(y.ChapterTimecodeStart);

            return firstStart.CompareTo(secondStart);

        }

        private void ExtractAttachments(MergePart segment)
        {
            using (var extractor = new AttachmentExtractor())
            {
                extractor.PerformExtraction(segment.Info, segment.Filename);
            }
        }

        private static string CreateAndChangeTo(string request)
        {
            // OK, change directory to make sure things are outputted there
            var oldPath = Environment.CurrentDirectory;
            var newPath = Path.Combine(oldPath, request);
            Directory.CreateDirectory(newPath);
            Environment.CurrentDirectory = newPath;
            return oldPath;
        }

        private void ExtractSplits(string baseFilename)
        {
            var oldPath = CreateAndChangeTo("splits");


            if (this._timecodes.Any())
            {
                var joined = string.Join(",", this._timecodes);
                Console.WriteLine(joined);
                var p = Process.Start("mkvmerge",
                    $"--no-chapters -o split-%03d.mkv \"{this._filename}\" --split timecodes:{joined}");

                p.WaitForExit();
            }
            else
            {
                // Must be sandwhiched and have no chances for splits... use the base
                File.Copy(baseFilename, "split-001.mkv");
            }

            Environment.CurrentDirectory = oldPath;
        }

    }
}