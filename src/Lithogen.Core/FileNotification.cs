using System;
using System.Diagnostics;

namespace Lithogen.Core
{
    [DebuggerDisplay("{NotificationType}, {Filename}")]
    public class FileNotification : IEquatable<FileNotification>
    {
        public FileNotificationType NotificationType { get; private set; }
        public string Filename { get; private set; }

        public FileNotification(FileNotificationType notificationType, string filename)
        {
            Filename = filename;
            NotificationType = notificationType;
        }

        public bool Equals(FileNotification other)
        {
            return other != null && 
                   NotificationType == other.NotificationType &&
                   Filename.Equals(other.Filename, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + NotificationType.GetHashCode();
                hash = hash * 23 + (Filename ?? "").ToLowerInvariant().GetHashCode();
                return hash;
            }
        }
    }
}
