using System.Diagnostics;
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
            proc.WaitForExit();

            var data = proc.StandardOutput.ReadToEnd();
            var mapper = new XmlMkvInfoSummaryMapper();
            var doc = mapper.DecodeStringIntoDocument(data);

            return new MkvInfo(doc);
        }
    }
}