namespace Lithogen.Core
{
    public enum FileNotificationType
    {
        /// <summary>
        /// A clean is required on this file (probably because it was deleted).
        /// </summary>
        Clean,

        /// <summary>
        /// A build is required on this file.
        /// </summary>
        Build
    }
}
