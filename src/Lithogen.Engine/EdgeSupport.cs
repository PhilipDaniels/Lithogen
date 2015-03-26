using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public static Func<object, Task<object>> GetStdoutHook()
        {
            var onMessage = (Func<object, Task<object>>)(async (msg) =>
            {
                return "STDOUT: " + (string)msg;
            });

            return onMessage;
        }

        public static Func<object, Task<object>> GetStderrHook()
        {
            var onMessage = (Func<object, Task<object>>)(async (msg) =>
            {
                return "STDERR: " + (string)msg;
            });

            return onMessage;
        }
    }
}
