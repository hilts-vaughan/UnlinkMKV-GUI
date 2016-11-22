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

    public class MergeJob
    {
        private readonly MergeOptions _options;
        private readonly string _filename;
        private List<Tuple<string, int>> _timecodes = new List<Tuple<string, int>>();
        private List<string> _segMapping = new List<string>();

        public MergeJob(MergeOptions options, string filename)
        {
            _options = options;
            _filename = filename;
        }

        public void PerformMerge()
        {
            Console.WriteLine("Starting the processing of the file: {0}", _filename);


            var loader = new MkvToolNixMkvInfoLoaderStrategy();
            var info = loader.FetchMkvInfo(_filename);

            if (info.IsFileLinked())
            {
                Console.WriteLine("File was deteremined to be linked so will begin process...");

                var workingDir = PathUtil.GetTemporaryDirectory();
                Environment.CurrentDirectory = workingDir;

                Console.WriteLine($"Your working directory will be: {workingDir}");

                // Step 1: Capture the segments that are the same in the directory
                var segments = SegmentUtility.GetMergePartsForFilename(_filename, info);

                // Step 2: Extract all attachments from the current segments ++ extract out the main attachments, too
                segments.ForEach(ExtractAttachments);

                // TODO: Later, the command line arguments can be built...

                // Step 3: Create the split for the segments; get the splitting ready
                SplitTimecode(info, segments);

                // Step 4: Extract the splits
                ExtractSplits(_filename);

                // Step 5: Let's rebuild the file... the timecodes can tell us quite a bit but let's use the fully
                // built "order"
                RebuildFile();

                Console.WriteLine("Job complete!");
            }
            else
            {
                Console.WriteLine("File was determined to not be linked. Ignoring.");
            }

            // TODO: Doing things here async is cool... thanks C# :)
            // await Task.Delay(1000);

             // TODO: Replace me the the proper steps
            // return new MergeResult("/mnt/media/Processed/inbox.mkv");
        }

        private void RebuildFile()
        {
            var proc = new ProcessStartInfo("mkvmerge");

            // Removing chapters options helps with the following issues:
            //  1. Ordered flags will get left behind sometimes, which is a nuissance
            //  2. Artifacted chapters are rarely useful when played back in something like Plex
            var parts = string.Join(" --no-chapters +", this._segMapping);

            proc.Arguments = $"--no-chapters -o output.mkv {parts}";
            var process = new Process();
            process.StartInfo = proc;
            process.Start();
            process.WaitForExit();
        }

        private void SplitTimecode(MkvInfo info, IList<MergePart> segments)
        {
            var prev = "00:00:00.000000000";

            // This is the index that would have to be "added" onto... we'd always have at least one split
            var splitIndex = 1;

            var englishChapters = info.Chapters.Editions[0].Chapters.Where(x => x.ChapterLanguage == "eng").ToList();

            // Should probably sort these chapters by timecode
            englishChapters.Sort(TimecodeComparator);

            foreach (var chapterAtom in englishChapters)
            {
               // A split is required if you're linked; otherwise you can forget it
                if (chapterAtom.IsLinked())
                {
                    if (prev != "00:00:00.000000000")
                    {
                        Console.WriteLine("segment to link!");
                        this._timecodes.Add(new Tuple<string, int>(prev, splitIndex));
                        splitIndex++;
                    }


                    var segment = segments.First(x => x.Info.SegmentUid.IsSame(chapterAtom.ReferencedSegmentUid));
                    this._segMapping.Add(segment.Filename);
                }
                else
                {
                    prev = chapterAtom.ChapterTimecodeEnd;
                    Console.WriteLine("split - original");


                    // We check the last string to handle the case where the user might have decided
                    // to place chapters next to each other, even if they are adjacenet in the split
                    var filename = $"splits/split-{splitIndex:D3}.mkv"; // for 3 digit splits
                    var lastFilename = this._segMapping.LastOrDefault();

                    if (filename != lastFilename)
                    {
                        this._segMapping.Add(filename);
                    }

                }
            }
        }

        private int TimecodeComparator(ChapterAtom x, ChapterAtom y)
        {
            var firstStart = TimeCodeToTimespan(x.ChapterTimecodeStart);
            var secondStart = TimeCodeToTimespan(y.ChapterTimecodeStart);

            return firstStart.CompareTo(secondStart);

        }

        private TimeSpan TimeCodeToTimespan(string timeCode)
        {
            var splits = timeCode.Split(':');
            var hour = splits[0];
            var minute = splits[1];
            var second = splits[2].Split('.')[0];

            return new TimeSpan(0, int.Parse(hour), int.Parse(minute), int.Parse(second));
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
            var timecodes = this._timecodes.Select(x => x.Item1);


            if (timecodes.Any())
            {
                var joined = string.Join(",", timecodes);
                Console.WriteLine(joined);
                var p = Process.Start("mkvmerge",
                    $"--no-chapters -o split-%03d.mkv {this._filename} --split timecodes:{joined}");

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