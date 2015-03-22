using System.Collections.Generic;
using System.IO;
using System.Linq;
using BassUtils;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class ViewFileProvider : IViewFileProvider
    {
        readonly IViewFileNameFilter ViewFileNameFilter;

        public ViewFileProvider(IViewFileNameFilter viewFileNameFilter)
        {
            ViewFileNameFilter = viewFileNameFilter.ThrowIfNull("viewFileNameFilter");
        }

        public IEnumerable<string> GetViewFiles(string directory)
        {
            directory.ThrowIfDirectoryDoesNotExist("directory");

            return from filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                   where !ViewFileNameFilter.ShouldIgnore(filename)
                   select filename;
        }
    }
}
