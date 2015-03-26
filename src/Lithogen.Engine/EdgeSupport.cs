using System.Collections.Generic;
using System.Diagnostics;

namespace Lithogen.Engine
{
    /// <summary>
    /// Simple class to represent a message from Stdout/Stderr as returned by
    /// the Edge outputhooker.
    /// </summary>
    [DebuggerDisplay("{Message}, IsError={IsError}")]
    public class NodeMessage
    {
        public bool IsError { get; private set; }
        public string Message { get; private set; }

        public NodeMessage(bool isError, string message)
        {
            IsError = isError;
            Message = message.Trim();
        }
    }

    public static class EdgeSupport
    {
        public static IEnumerable<NodeMessage> UnpackMessages(dynamic messages)
        {
            var msgs = new List<NodeMessage>();

            foreach (var message in messages)
            {
                var msg = new NodeMessage(message.stream[0] == 'e', message.message);
                msgs.Add(msg);
            }

            return msgs;
        }
    }
}
