using System.Collections.Generic;
using System.IO;
using System.Text;
using BassUtils;

namespace Lithogen.Core
{
    public static class FileUtilities
    {
        /// <summary>
        /// Returns the penultimate extension of a file. For example, Foo.md.html returns md,
        /// and Foo.html returns Foo.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>The penultimate extension.</returns>
        public static string GetPenultimateExtension(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return Path.GetExtension(fileName);
        }

        /// <summary>
        /// Returns a list of all files in the directory and all subdirectories.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <returns>List of all files found.</returns>
        public static IEnumerable<string> GetAllFilesInDirectoryRecursive(string directory)
        {
            directory.ThrowIfDirectoryDoesNotExist("directory");
            return Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Returns all the DLLs in a directory. Does not search subdirectories.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <returns>List of dlls.</returns>
        public static IEnumerable<string> GetDlls(string directory)
        {
            directory.ThrowIfDirectoryDoesNotExist("directory");
            string[] dlls = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);
            return dlls;
        }

        /// <summary>
        /// Writes text to a file, ensuring that the file is UTF-8 and includes
        /// the preamble. The file will be overwritten if it already exists.
        /// </summary>
        /// <param name="fileName">The filename to write to.</param>
        /// <param name="text">The text to write.</param>
        public static void WriteFileWithUtf8Preamble(string fileName, string text)
        {
            var enc = new UTF8Encoding(true);

            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var sw = new StreamWriter(fs, enc))
            {
                sw.Write(text);
            }
        }
    }
}
