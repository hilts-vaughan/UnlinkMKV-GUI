using System.IO;
using System.Threading.Tasks;
using UnlinkMKV_GUI.merge;

namespace UnlinkMKV_GUI
{
    public interface IMkvJobMerger
    {
        Task<MergeResult> ExecuteJob(TextWriter logger, string source, string dest);
    }
}