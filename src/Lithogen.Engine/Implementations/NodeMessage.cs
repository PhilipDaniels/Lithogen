using System.Diagnostics;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Simple DTO class to represent a message from Stdout/Stderr as returned by
    /// the Edge output stream hooker.
    /// </summary>
    [DebuggerDisplay("{Message}, IsError={IsError}")]
    public class NodeMessage
    {
        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// If true, this was a message from stderr, otherwise it was from stdout.
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// Create a new NodeMessage object.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="isError">If true, this was a message from stderr, otherwise it was from stdout.</param>
        public NodeMessage(string message, bool isError)
        {
            Message = message.Trim(); 
            IsError = isError;
        }
    }
}
