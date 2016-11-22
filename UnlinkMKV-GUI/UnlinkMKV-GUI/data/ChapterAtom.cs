using System.Xml.Linq;

namespace UnlinkMKV_GUI.data
{
    public class ChapterAtom
    {
        public ChapterAtom(XContainer chapterNode)
        {
            ChapterTimecodeStart = chapterNode.Element("ChapterTimeStart")?.Value;
            ChapterTimecodeEnd = chapterNode.Element("ChapterTimeEnd")?.Value;
            ChapterName = chapterNode.Element("ChapterDisplay")?.Element("ChapterString")?.Value;
            ChapterLanguage = chapterNode.Element("ChapterDisplay")?.Element("ChapterLanguage")?.Value;

            var segmentNode = chapterNode.Element("ChapterSegmentUID");
            if (segmentNode != null)
            {
                ReferencedSegmentUid = new MkvSegmentUid(segmentNode.Value);
            }
        }

        public string ChapterTimecodeEnd { get; set; }
        public string ChapterTimecodeStart { get; }
        public string ChapterName { get; }
        public string ChapterLanguage { get; }

        /// <summary>
        /// Represents the segment UID that this chapter might actually be linked to, if it is linked.
        /// </summary>
        public MkvSegmentUid ReferencedSegmentUid { get; private set; }

        /// <summary>
        /// Returns whether or not this chapter atom is linked.
        /// </summary>
        /// <returns></returns>
        public bool IsLinked()
        {
            return ReferencedSegmentUid != null;
        }

    }
}