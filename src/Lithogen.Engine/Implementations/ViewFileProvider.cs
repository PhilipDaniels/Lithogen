using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

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

            return from filename in FileUtils.GetAllFilesInDirectoryRecursive(directory)
                   where !ViewFileNameFilter.ShouldIgnore(filename)
                   select filename;
        }
    }
}
