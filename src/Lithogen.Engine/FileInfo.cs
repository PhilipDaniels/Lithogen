using Lithogen.Core;
using System;
using System.IO;

namespace Lithogen.Engine
{
    /// <summary>
    /// Represents the file system information about a file.
    /// We have our own type, instead of using the <code>FileInfo</code>
    /// class, because we want it to be immutable.
    /// </summary>
    public class FileInfo : IFileInfo
    {
        public string FullName { get; private set; }
        public string DirectoryName { get; private set; }
        public string Name { get; private set; }
        public string Extension { get; private set; }
        public long Length { get; private set; }

        public FileAttributes Attributes { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime CreationTimeUtc { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime LastAccessTimeUtc { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public DateTime LastWriteTimeUtc { get; private set; }

        public FileInfo(string fileName)
        {
            var fi = new System.IO.FileInfo(fileName);

            FullName = fi.FullName;
            DirectoryName = fi.DirectoryName;
            Name = fi.Name;
            Extension = FileUtils.CleanExtension(fi.Extension);
            Attributes = fi.Attributes;
            Length = fi.Length;
            CreationTime = fi.CreationTime;
            CreationTimeUtc = fi.CreationTimeUtc;
            LastAccessTime = fi.LastAccessTime;
            LastAccessTimeUtc = fi.LastAccessTimeUtc;
            LastWriteTime = fi.LastWriteTime;
            LastWriteTimeUtc = fi.LastWriteTimeUtc;
        }
    }
}
