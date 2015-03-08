using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lithogen.Core
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the full manifest filename of a manifest resource. The <paramref name="fileName" />
        /// is of the format "folder.folder.filename.ext" and is case sensitive.
        /// </summary>
        /// <param name="assembly">The assembly in which to form the filename.</param>
        /// <param name="fileName">Filename you want to refer to.</param>
        /// <returns>Fully qualified manifest resource filename.</returns>
        public static string GetResourceFileName(this Assembly assembly, string fileName)
        {
            assembly.ThrowIfNull("assembly");
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            string name = String.Concat(assembly.GetName().Name, ".", fileName);
            return name;
        }

        /// <summary>
        /// Gets an open stream on the specified embedded resource. It is the
        /// caller's responsibility to call Dispose() on the stream.
        /// The filename is of the format "folder.folder.filename.ext"
        /// and is case sensitive.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the Stream.</param>
        /// <param name="fileName">Filename whose contents you want.</param>
        /// <returns>Stream object.</returns>
        public static Stream GetResourceStream(this Assembly assembly, string fileName)
        {
            assembly.ThrowIfNull("assembly");
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            string name = assembly.GetResourceFileName(fileName);
            Stream s = assembly.GetManifestResourceStream(name);
            return s;
        }

        /// <summary>
        /// Get the contents of an embedded file as a string.
        /// The filename is of the format "folder.folder.filename.ext"
        /// and is case sensitive.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the file.</param>
        /// <param name="filename">Filename whose contents you want.</param>
        /// <returns>String object.</returns>
        public static string GetResourceAsString(this Assembly assembly, string fileName)
        {
            assembly.ThrowIfNull("assembly");
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            using (Stream s = GetResourceStream(assembly, fileName))
            using (StreamReader sr = new StreamReader(s))
            {
                string fileContents = sr.ReadToEnd();
                return fileContents;
            }
        }

        public static bool DoesNotReferenceAssemblies(this Assembly assembly, params string[] blacklist)
        {
            return assembly.DoesNotReferenceAssemblies((IEnumerable<string>)blacklist);
        }

        public static bool DoesNotReferenceAssemblies(this Assembly assembly, IEnumerable<string> blacklist)
        {
            assembly.ThrowIfNull("assembly");
            blacklist.ThrowIfNull("blacklist");

            var references = assembly.GetReferencedAssemblies();

            foreach (var asmName in references)
            {
                foreach (var partialName in blacklist)
                {
                    if (asmName.Name.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) != -1)
                        return false;
                }
            }

            return true;
        }
    }
}
