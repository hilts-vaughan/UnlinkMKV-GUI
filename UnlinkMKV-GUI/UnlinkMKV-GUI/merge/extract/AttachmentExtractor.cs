using System.Diagnostics;
using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge.extract
{
    public class AttachmentExtractor: BaseMatroskaExtractor
    {
        public override void PerformExtraction(MkvInfo info, string file)
        {
            // Swap to attachments directory
            CreateAndChangeTo("attachments");

            var segment = new MergePart(info, file);

            var attachmentCount = segment.Info.Attachments.Count;
            var joinedCounts = "";
            for (var i = 0; i < attachmentCount; i++)
            {
                joinedCounts  += $"{i + 1} ";
            }
            var proc = Process.Start("mkvextract", $"attachments \"{segment.Filename}\" {joinedCounts}");
            proc.WaitForExit();
        }
    }
}