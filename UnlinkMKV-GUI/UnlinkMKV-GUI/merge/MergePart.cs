using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge
{
    public class MergePart
    {
        public MergePart(MkvInfo info, string filename)
        {
            Info = info;
            Filename = filename;
        }

        public MkvInfo Info { get; }
         public string Filename { get; }
    }
}