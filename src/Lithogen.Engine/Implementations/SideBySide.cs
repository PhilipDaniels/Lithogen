using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Side by side files are .json or .yaml files with the same name as the main file.
    /// Side by side files can start with a leading "_", which prevents them from being
    /// copied to the output directory.
    /// </summary>
    public class SideBySide : ISideBySide
    {
        /// <summary>
        /// Returns the default set of side-by-side extensions.
        /// These are used if you don't pass any in.
        /// </summary>
        public IEnumerable<string> DefaultSideBySideExtensions
        {
            get
            {
                return new string[] { "json", "yaml" };
            }
        }

        /// <summary>
        /// Retrieves a list of the side-by-side files that exist for a given file.
        /// List may be empty.
        /// </summary>
        /// <param name="fileName">The file.</param>
        /// <param name="sideBySideExtension">Expected extension, e.g. "yaml" or "json".</param>
        /// <returns>List of existing files.</returns>
        public IEnumerable<string> GetSideBySideFiles(string fileName, string sideBySideExtension)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");
            sideBySideExtension.ThrowIfNullOrWhiteSpace("sideBySideExtension");

            // Convention: foo.cshtml can have foo.yaml and _foo.yaml in that order.
            string sbsFile = Path.ChangeExtension(fileName, sideBySideExtension);
            if (File.Exists(sbsFile))
                yield return sbsFile;

            string dir = Path.GetDirectoryName(sbsFile);
            string basename = "_" + Path.GetFileName(sbsFile);
            sbsFile = Path.Combine(dir, basename);
            if (File.Exists(sbsFile))
                yield return sbsFile;
        }

        /// <summary>
        /// Returns true if the file is a side-by-side file. This is determined by checking the file extension
        /// against a list of possible extensions. No checking is done that a corresponding main file
        /// exists, so this is a fast method.
        /// </summary>
        /// <param name="possibleSideBySideFileName">The filename you want to check.</param>
        /// <param name="sideBySideExtensions">Set of extensions to check against. Normally you
        /// would leave this blank to use the defaults.</param>
        /// <returns>True if this is a side-by-side file, false otherwise.</returns>
        public bool IsSideBySideFile(string possibleSideBySideFileName, params string[] sideBySideExtensions)
        {
            possibleSideBySideFileName.ThrowIfNullOrWhiteSpace("possibleSideBySideFileName"); 
            
            if (sideBySideExtensions == null || sideBySideExtensions.Count() == 0)
                sideBySideExtensions = DefaultSideBySideExtensions.ToArray();

            string fileExt = FileUtils.GetCleanExtension(possibleSideBySideFileName);
            return sideBySideExtensions.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the corresponding main file for a side-by-side file. Will return null if <paramref name="possibleSideBySideFileName"/>
        /// is not actually a side-by-side file or if there is no corresponding main file.
        /// If a file is returned, it is guaranteed to exist on disk (so the method can be slow because it
        /// has to do a file search).
        /// </summary>
        /// <param name="possibleSideBySideFileName">The filename you want to check.</param>
        /// <param name="sideBySideExtensions">Set of extensions to check against. Normally you
        /// would leave this blank to use the defaults.</param>
        /// <returns>The name of the main file if it exists, false otherwise.</returns>
        public string GetMainFile(string possibleSideBySideFileName, params string[] sideBySideExtensions)
        {
            possibleSideBySideFileName.ThrowIfNullOrWhiteSpace("possibleSideBySideFileName");
            if (sideBySideExtensions == null || sideBySideExtensions.Count() == 0)
                sideBySideExtensions = DefaultSideBySideExtensions.ToArray();

            if (IsSideBySideFile(possibleSideBySideFileName, sideBySideExtensions))
            {
                string dir = Path.GetDirectoryName(possibleSideBySideFileName);
                string pattern = Path.ChangeExtension(Path.GetFileName(possibleSideBySideFileName), "*");
                string mainFile = GetMainFileImpl(dir, pattern, possibleSideBySideFileName, sideBySideExtensions);
                if (mainFile != null)
                {
                    return mainFile;
                }
                else
                {
                    if (pattern.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    {
                        pattern = pattern.Substring(1);
                        return GetMainFileImpl(dir, pattern, possibleSideBySideFileName, sideBySideExtensions);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        string GetMainFileImpl(string dir, string pattern, string possibleSideBySideFileName, params string[] sideBySideExtensions)
        {
            var filesOnDisk = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
            foreach (var fileOnDisk in filesOnDisk)
            {
                if (IsSideBySideFile(fileOnDisk, sideBySideExtensions))
                    continue;
                if (fileOnDisk.Equals(possibleSideBySideFileName, StringComparison.OrdinalIgnoreCase))
                    continue;
                return fileOnDisk;
            }

            return null;
        }
    }
}
