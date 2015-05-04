using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    public interface IViewFileProvider
    {
        IEnumerable<string> GetViewFiles(string directory);
    }
}
