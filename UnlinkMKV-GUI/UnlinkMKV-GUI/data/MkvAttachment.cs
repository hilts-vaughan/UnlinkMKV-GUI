using System.Xml.Linq;

namespace UnlinkMKV_GUI.data
{
    public class MkvAttachment
    {
        public MkvAttachment(XElement attachmentNode)
        {
            Filename = attachmentNode.Element("FileName")?.Value;
            MimeType = attachmentNode.Element("MimeType")?.Value;
            FileUid = attachmentNode.Element("FileUID")?.Value;
            FileDataSize = int.Parse((attachmentNode.Element("FileDataSize")?.Value));
        }

        public string Filename { get; }
        public string MimeType { get; }
        public string FileUid { get; }
        public int FileDataSize { get; }
    }
}