using System.Collections.Generic;
using System.Diagnostics;

namespace Gitcheatsheet.Model
{
    [DebuggerDisplay("{Title}")]
    public class Section
    {
        public string Title { get; private set; }
        public List<SubSection> SubSections { get; private set; }

        public Section(string title)
        {
            Title = title;
            SubSections = new List<SubSection>();
        }

        public SubSection AddSubSection(string title)
        {
            var subsec = new SubSection(title);
            SubSections.Add(subsec);
            return subsec;
        }
    }
}
