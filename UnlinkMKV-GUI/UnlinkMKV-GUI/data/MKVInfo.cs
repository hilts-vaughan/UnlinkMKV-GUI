using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UnlinkMKV_GUI.data
{
    /// <summary>
    /// Represents some basic data around a MKV file that has been processed.
    /// </summary>
    public class MkvInfo
    {
        public MkvInfo(XDocument source)
        {
            // Build up our data model to work with here... and then create a CLI utility that works everywhere. :)
            // Probably can just make it take one file and then xargs them together in a chain

            var root = source.XPathSelectElement("MKVInfo/SegmentSize/SegmentInformation");
            var outsideRoot = source.XPathSelectElement("MKVInfo/SegmentSize");

            if (root != null)
            {
                Writer = root.Element("WritingApplication")?.Value;

                // Create attachments
                Attachments = new List<MkvAttachment>();
                var attachmentNodes = outsideRoot.Element("Attachments").Elements();
                foreach (var attachmentNode in attachmentNodes)
                {
                    Attachments.Add(new MkvAttachment(attachmentNode));
                }

                // Create chapters
                var chapters = outsideRoot.Element("Chapters");
                if (chapters != null)
                {
                    Chapters = new Chapter(chapters);
                }

                SegmentUid = new MkvSegmentUid(source.XPathSelectElement("MKVInfo/SegmentSize/SegmentInformation/SegmentUID").Value);
            }
            else
            {
                throw new Exception("Failed to find the node that was needed.");
            }

        }

        public DateTime DateAuthored { get;  }
        public string Writer { get;  }
        public List<MkvAttachment> Attachments { get; }
        public MkvSegmentUid SegmentUid { get; }
        public Chapter Chapters { get;  }
        public TimeSpan Duration { get; set; }

        public bool IsFileLinked()
        {
            return Chapters.Editions.Any(x => x.Chapters.Any(y => y.IsLinked()));
        }

    }
}