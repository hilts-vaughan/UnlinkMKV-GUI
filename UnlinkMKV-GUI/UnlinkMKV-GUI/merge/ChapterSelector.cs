using System.Collections.Generic;
using System.Linq;
using UnlinkMKV_GUI.data;

namespace UnlinkMKV_GUI.merge
{
    /// <summary>
    /// Selects chapters that are required for processing based on some rules.
    /// As time goes on, I am sure some releases will have some hacks that are required to be applied to them that
    /// we can probably have this selector apply. These are problably distributed in the form of decorators or something
    /// that can be compiled and ran or just simply C# files with logic that can be additionally tagged and downloaded
    /// from the web.
    /// </summary>
    public class ChapterSelector
    {
        IList<ChapterAtom> GetRelevantChapters(IList<ChapterAtom> chapters)
        {
            // For now, just strip off undefined chapter languages
            return chapters.Where(x => x.ChapterLanguage != "und").ToList();
        }

    }
}