using System.Collections.Generic;
using System.Diagnostics;

namespace Gitcheatsheet.Model
{
    [DebuggerDisplay("{Title}")]
    public class SubSection
    {
        public string Title { get; private set; }
        public List<Cheat> Cheats { get; private set; }

        public SubSection(string title)
        {
            Title = title;
            Cheats = new List<Cheat>();
        }

        public Cheat AddCheat(string affected, string text, string hint, string url = null)
        {
            var cheat = new Cheat(affected, text, hint, url);
            Cheats.Add(cheat);
            return cheat;
        }
    }
}
