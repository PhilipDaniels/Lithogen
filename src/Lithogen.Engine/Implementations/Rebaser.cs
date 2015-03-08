using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.IO;

namespace Lithogen.Engine.Implementations
{
    public class Rebaser : IRebaser
    {
        readonly ISettings TheSettings;

        public Rebaser(ISettings settings)
        {
            TheSettings = settings.ThrowIfNull("settings");
        }

        public virtual string RebaseFileNameIntoOutputDirectory(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            if (filename.StartsWith(TheSettings.LithogenWebsiteDirectory, StringComparison.InvariantCultureIgnoreCase))
                return filename;

            string subPath;
            if (filename.StartsWith(TheSettings.ViewsDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                // Files in the view folder get moved up a level.
                subPath = filename.Replace(TheSettings.ViewsDirectory, "", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                subPath = filename.Replace(TheSettings.ProjectDirectory, "", StringComparison.InvariantCultureIgnoreCase);
            }

            subPath = subPath.TrimStart(new char[] { '\\', '/' });

            string rebasedName = Path.Combine(TheSettings.LithogenWebsiteDirectory, subPath);
            return rebasedName;
        }

        public virtual string GetPathToRoot(string containingFilename)
        {
            return "";


            // ResolveRoot("C:\temp\Lithogen\Views\foo")                -> "./"
            // ResolveRoot("C:\temp\Lithogen\Views\sub\foo")            -> "../"
            // ResolveRoot("C:\temp\Lithogen\content\something.css")    -> "../"

            /*
             * Given the path ~ in a file, we want to determine the number of .. we
             * need to reach the root directory. This will be slightly different for
             * views (which are moved up one level) vs content and JS.
             * 
             * The filename must be within one of the special project directories.
             * Determine how many times you have to call Path.GetDirectory() until your reach the project directory.
             * Subtract 1 from that number if a view file.
             * 
             * Return the appropriate number of ..
             * 
             * THEROOT(~)
             */
        }

        public virtual string ReplaceRootsInFile(string containingFilename, string contents)
        {
            return "";
        }
    }
}
