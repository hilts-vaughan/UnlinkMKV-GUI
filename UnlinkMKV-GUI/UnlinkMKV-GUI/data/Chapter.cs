using System.Collections.Generic;
using System.Xml.Linq;

namespace UnlinkMKV_GUI.data
{
    public class Chapter
    {
        public Chapter(XContainer node)
        {
            Editions = new List<MkvEdition>();
            var editions = node.Elements("EditionEntry");
            foreach (var edition in editions)
            {
                Editions.Add(new MkvEdition(edition));
            }
        }

        public List<MkvEdition> Editions { get; }
    }
}