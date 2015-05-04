using System;
using System.IO;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents the file system information about a file.
    /// We have our own type, instead of using the <code>FileInfo</code>
    /// class, because we want it to be immutable.
    /// </summary>
    public interface IFileInfo
    {
        string FullName { get; }
        string DirectoryName { get; }
        string Name { get; }
        string Extension { get; }
        long Length { get; }

        FileAttributes Attributes { get; }

        DateTime CreationTime { get; }
        DateTime CreationTimeUtc { get; }
        DateTime LastAccessTime { get; }
        DateTime LastAccessTimeUtc { get; }
        DateTime LastWriteTime { get; }
        DateTime LastWriteTimeUtc { get; }
    }
}
