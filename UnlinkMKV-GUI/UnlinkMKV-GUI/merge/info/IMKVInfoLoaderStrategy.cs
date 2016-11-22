using System.Security.Cryptography.X509Certificates;
using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge.info
{
    public interface IMkvInfoLoaderStrategy
    {
        MkvInfo FetchMkvInfo(string file);
    }
}