using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UnlinkMKV_GUI.data
{
    public class MkvEdition
    {
        public MkvEdition(XContainer editionNode)
        {
            Chapters = new List<ChapterAtom>();
            var chapterNodes = editionNode.Elements().Where(x => x.Name == "ChapterAtom");
            foreach (var chapterNode in chapterNodes)
               Chapters.Add(new ChapterAtom(chapterNode));

            // We just assume in good faaith here that this is all valid
            var isDefaultBitFlag = int.Parse(editionNode.Element("EditionFlagDefault")?.Value);
            if (isDefaultBitFlag == 1)
            {
                IsDefault = true;
            }
        }

        public bool IsDefault { get; }
        public int EditionUid { get;}

        public List<ChapterAtom> Chapters { get; private set; }
    }
}