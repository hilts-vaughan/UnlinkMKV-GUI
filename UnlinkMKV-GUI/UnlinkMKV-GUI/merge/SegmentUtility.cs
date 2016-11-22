using System.Collections.Generic;
using System.IO;
using UnlinkMKV_GUI.data;
using UnlinkMKV_GUI.merge.info;

namespace UnlinkMKV_GUI.merge
{
    public static class SegmentUtility
    {
        public static List<MergePart> GetMergePartsForFilename(string filename, MkvInfo baseInfo)
        {
            var result = new List<MergePart>();

            var candidates = Directory.GetFiles(Path.GetDirectoryName(filename), "*.mkv");
            var loader = new MkvToolNixMkvInfoLoaderStrategy();
            foreach (var candidate in candidates)
            {
                if(candidate == filename) continue;

                var info = loader.FetchMkvInfo(candidate);
                var isSafe = false;
                baseInfo.Chapters.Editions[0].Chapters.ForEach((x => isSafe |= info.SegmentUid.IsSame(x.ReferencedSegmentUid)));
                if (isSafe)
                {
                  result.Add(new MergePart(info, candidate));
                }
            }

            return result;
        }
    }
}