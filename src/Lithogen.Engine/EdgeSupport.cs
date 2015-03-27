using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lithogen.Core.Interfaces;

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

    public class EdgeSupport
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

        readonly ILogger Logger;
        public EdgeSupport(ILogger logger)
        {
            Logger = logger;
        }

        public Func<object, Task<object>> GetStdoutHook()
        {
            var onMessage = (Func<object, Task<object>>)(async (msg) =>
            {
                return Task.Run(() => Logger.LogMessage((string)msg));
            });

            return onMessage;
        }

        public Func<object, Task<object>> GetStderrHook()
        {
            var onMessage = (Func<object, Task<object>>)(async (msg) =>
            {
                return Task.Run(() => Logger.LogError((string)msg));
            });

            return onMessage;




            //return (Func<object, Task<object>>)((msg) => { return (string)msg; });

            //return (msg) =>
            //{
            //    return Task.Run(() => Logger.LogError((string)msg));
            //    //return Task.Run(() => (object)("STDERR: " + (string)msg));
            //};

            //return onMessage;
        }

        // This one needs a cast.
        public static Task<object> GetStdoutHook2(object message)
        {
            return Task.Run(() => (object)("STDOUT2: " + (string)message));
        }
    }
}
