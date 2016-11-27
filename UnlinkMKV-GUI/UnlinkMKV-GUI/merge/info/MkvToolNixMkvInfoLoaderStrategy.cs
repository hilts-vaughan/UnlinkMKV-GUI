using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using UnlinkMKV_GUI.data;
using UnlinkMKV_GUI.data.xml;

namespace UnlinkMKV_GUI.merge.info
{
    public class MkvToolNixMkvInfoLoaderStrategy : IMkvInfoLoaderStrategy
    {
        public MkvInfo FetchMkvInfo(string file)
        {
            var proc = new Process {StartInfo = new ProcessStartInfo("mkvinfo", file) {RedirectStandardOutput = true, UseShellExecute =  false}};

            if (proc == null)
            {
                throw new MkvNixException("Failed to find / start mkvinfo");
            }

            proc.Start();
            var data = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            var mapper = new XmlMkvInfoSummaryMapper();
            var doc = mapper.DecodeStringIntoDocument(data);

            // Append some extract MediaInfo data that we might need... :(
            var mediaInfoProc = new Process
            {
                StartInfo =
                    new ProcessStartInfo("mediainfo", $"--Inform=\"Video;%Duration/String3%\" {file}") {RedirectStandardOutput = true, UseShellExecute = false}
            };

            mediaInfoProc.Start();
            var mediaInfoStr = mediaInfoProc.StandardOutput.ReadToEnd();
            mediaInfoProc.WaitForExit();

            // mediainfo > file > duration... weird format
            var mkvInfo = new MkvInfo(doc) {Duration = TimeCodeUtil.TimeCodeToTimespan(mediaInfoStr)};
            return mkvInfo;
        }
    }
}