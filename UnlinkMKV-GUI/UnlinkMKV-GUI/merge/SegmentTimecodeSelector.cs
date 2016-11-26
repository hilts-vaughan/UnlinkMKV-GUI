using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge
{
    public class SegmentTimecodeSelector
    {

        public class SelectorDto
        {
            public SelectorDto(List<string> segmentMap, List<string> timecodes)
            {
                SegmentMap = segmentMap;
                Timecodes = timecodes;
            }

            public List<string> SegmentMap { get; private set; }
            public List<string> Timecodes { get; private set; }
        }

        private List<string> _segMapping = new List<string>();
        private List<string> _timecodes = new List<string>();


        public SelectorDto GetTimecodeAndSegments(MkvInfo info, IList<MergePart> segments, bool forceReorder)
        {
            this._segMapping.Clear();
            this._timecodes.Clear();

            // All chapters
            var englishChapters = info.Chapters.Editions[0].Chapters.ToList();
            englishChapters = englishChapters.Distinct().ToList();

            var increasesLinearly = AreChapterStartTimesLinearlyIncreasing(englishChapters);

            if (!increasesLinearly || forceReorder)
            {
                // This means that we'll have to reconstruct the timecodes based on the order in the file
                // as a best guess.. this would require MkvInfo and can be kind of dangerous as it assumes
                // all are sequential, but it's the best we can do in many cases.
                Console.WriteLine("Forced to reorder due to broken file...");
                englishChapters = ReOrderChaptersAndTimeCodes(segments, info, englishChapters);
            }

            SplitTimecode(info, segments,englishChapters);

            return new SelectorDto(this._segMapping, this._timecodes);
        }

        private void SplitTimecode(MkvInfo info, IList<MergePart> segments, IList<ChapterAtom> chaptersInOrder)
        {
            var prev = "00:00:00.000000000";

            // This is the index that would have to be "added" onto... we'd always have at least one split
            var splitIndex = 1;

            foreach (var chapterAtom in chaptersInOrder)
            {
                // A split is required if you're linked; otherwise you can forget it
                if (chapterAtom.IsLinked())
                {
                    if (prev != "00:00:00.000000000")
                    {
                        Console.WriteLine("segment to link!");
                        this._timecodes.Add(prev);
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
                    // which would happen if the episode is split into two parts (Part A + Part B, for example)
                    var filename = $"splits/split-{splitIndex:D3}.mkv"; // for 3 digit splits
                    var lastFilename = this._segMapping.LastOrDefault();

                    if (filename != lastFilename)
                    {
                        this._segMapping.Add(filename);
                    }

                }
            }
        }

        private bool AreChapterStartTimesLinearlyIncreasing(IList<ChapterAtom>  chapters)
        {
            var prev = TimeCodeUtil.TimeCodeToTimespan(chapters.First().ChapterTimecodeStart);
            foreach (var chapter in chapters)
            {
                var timeSpan = TimeCodeUtil.TimeCodeToTimespan(chapter.ChapterTimecodeStart);
                if (timeSpan < prev)
                {
                    return false;
                }
                prev = timeSpan;
            }
            return true;
        }

        private List<ChapterAtom> ReOrderChaptersAndTimeCodes(IList<MergePart> segmentParts, MkvInfo baseInfo, List<ChapterAtom> chapters)
        {
            var prev = new TimeSpan();
            foreach (var chapter in chapters)
            {
                // Base info unless we say otherwise...
                MkvInfo info = null;

                if (chapter.ReferencedSegmentUid != null)
                {
                    var segment = segmentParts.First(x => x.Info.SegmentUid.IsSame(chapter.ReferencedSegmentUid));
                    info = segment.Info;

                    chapter.ChapterTimecodeStart = TimeCodeUtil.TimespanToTimeCode(prev);
                    prev = prev.Add(info.Duration);
                    chapter.ChapterTimecodeEnd = TimeCodeUtil.TimespanToTimeCode(prev);
                }
                else
                {
                    Console.WriteLine("Skipping a chapter from main file; it is presumed to be correct... but it may not be~!");

                    // Hand over; hopefully it's correct...
                    prev = TimeCodeUtil.TimeCodeToTimespan(chapter.ChapterTimecodeEnd);
                }
            }

            return chapters;
        }
    }
}