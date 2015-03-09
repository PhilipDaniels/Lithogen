using System;
using System.Diagnostics;

namespace Lithogen.Core
{
    [DebuggerDisplay("{NotificationType}, {FileName}")]
    public class FileNotification : IEquatable<FileNotification>
    {
        public FileNotificationType NotificationType { get; private set; }
        public string FileName { get; private set; }

        public FileNotification(FileNotificationType notificationType, string fileName)
        {
            FileName = fileName;
            NotificationType = notificationType;
        }

        public bool Equals(FileNotification other)
        {
            return other != null && 
                   NotificationType == other.NotificationType &&
                   FileName.Equals(other.FileName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + NotificationType.GetHashCode();
                hash = hash * 23 + (FileName ?? "").ToUpperInvariant().GetHashCode();
                return hash;
            }
        }
    }
}
