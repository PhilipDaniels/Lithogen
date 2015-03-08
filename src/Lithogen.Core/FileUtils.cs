using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lithogen.Core
{
    public static class FileUtils
    {
        /// <summary>
        /// Deletes all files and subdirectories in a directory, but leaves the directory.
        /// The directory does not need to exist (this is a no-op).
        /// </summary>
        /// <param name="directory">The directory to delete.</param>
        public static void DeleteDirectoryContents(string directory)
        {
            directory.ThrowIfNullOrWhiteSpace("directory");

            if (!Directory.Exists(directory))
                return;

            foreach (var file in Directory.EnumerateFiles(directory))
                File.Delete(file);

            foreach (var dir in Directory.EnumerateDirectories(directory))
            {
                Directory.Delete(dir, true);
            }
        }

        /// <summary>
        /// Creates a directory if it does not exist (and returns true).
        /// If the directory already exists, returns false.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        /// <returns>True if the directory was created, false if it already existed.</returns>
        public static bool EnsureDirectory(string directory)
        {
            directory.ThrowIfNullOrWhiteSpace("directory");

            if (Directory.Exists(directory))
            {
                return false;
            }
            else
            {
                Directory.CreateDirectory(directory);
                return true;
            }
        }

        /// <summary>
        /// Creates the parent directory of a file if it does not exist (and returns true).
        /// If the directory already exists, returns false.
        /// </summary>
        /// <param name="filename">The filename whose parent directory is to be created.</param>
        /// <returns>True if the directory was created, false if it already existed.</returns>
        public static bool EnsureParentDirectory(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            string parentDir = Path.GetDirectoryName(filename);
            return FileUtils.EnsureDirectory(parentDir);
        }

        /// <summary>
        /// Returns the penultimate extension of a file. For example, Foo.md.html returns md,
        /// and Foo.html returns Foo.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The penultimate extension.</returns>
        public static string GetPenultimateExtension(string filename)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            return Path.GetExtension(filename);
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
        /// <param name="filename">The filename to write to.</param>
        /// <param name="text">The text to write.</param>
        public static void WriteFileWithUtf8Preamble(string filename, string text)
        {
            var enc = new UTF8Encoding(true);

            using (var fs = new FileStream(filename, FileMode.Create))
            using (var sw = new StreamWriter(fs, enc))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// Extracts a "clean" extension from a filename. By default, Path.GetExtension
        /// returns extensions with leading "." characters. This method removes them.
        /// </summary>
        /// <param name="filename">Filename to get extension of.</param>
        /// <returns>Cleaned extension.</returns>
        public static string GetCleanExtension(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");
            string extension = CleanExtension(Path.GetExtension(filename));
            return extension;
        }

        /// <summary>
        /// Cleans up an extension by trimming any leading '.', which the Path
        /// methods often leave on. e.g. ".cshtml" becomes "cshtml".
        /// </summary>
        /// <param name="extension">The extension to trim.</param>
        /// <returns>Cleaned extension.</returns>
        public static string CleanExtension(string extension)
        {
            extension.ThrowIfNull("extension");

            extension = extension.Trim().TrimStart('.').Trim();
            return extension;
        }
    }
}
