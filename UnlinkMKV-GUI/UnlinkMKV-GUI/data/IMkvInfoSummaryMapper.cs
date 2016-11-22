using System.Xml.Linq;

namespace UnlinkMKV_GUI.data
{
    public interface IMkvInfoSummaryMapper
    {
        XDocument DecodeStringIntoDocument(string stringSource);
    }
}