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

        
        
        
        
        readonly ILogger TheLogger;

        public EdgeSupport(ILogger logger)
        {
            TheLogger = logger;
        }


        public Func<object, Task<object>> GetStdoutHook()
        {
            var onMessage = (Func<object, Task<object>>)(async (msg) =>
            {
                var t = Task.Run(() => TheLogger.LogMessage((string)msg));
                await t;
                return t;
            });

            return onMessage;
        }
       
        //async void StdOut(string message)
        //{
        //    await Task.Run(() => TheLogger.LogMessage(message));
        //}

        //public Func<object, Task<object>> GetStdoutHook()
        //{
        //    Func<object, Task<object>> f = (msg) => {
        //        //return await
        //        //return Task.Run(() => TheLogger.LogMessage((string)msg));
        //    };

        //    //return Task.Run((object msg) => TheLogger.LogMessage((string)msg));
        //    //return Task.Run(() => (object)("STDOUT2: " + (string)message));
        //}
        
        
        
        /*
         * 
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
        */

    }
}
