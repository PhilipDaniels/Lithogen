using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class EdgeHelper : IEdgeHelper
    {
        readonly ISettings TheSettings;
        readonly ILogger TheLogger;

        public EdgeHelper(ILogger logger, ISettings settings)
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
        }

        /// <summary>
        /// Replace the special token that indicates the folder where the node installation
        /// is for a particular project.
        /// </summary>
        /// <param name="javascript">The javascript to do the replacement in.</param>
        /// <returns>New javascript.</returns>
        public string ReplaceLithogenNodeRoot(string javascript)
        {
            javascript.ThrowIfNull("javascript");

            string dir = TheSettings.ProjectLithogenDirectory;
            dir = dir.Replace("\\", "/");
            javascript = javascript.Replace("PROJLITHDIR", dir);
            return javascript;
        }

        /// <summary>
        /// Creates a new ExpandoObject that contains two properties designed
        /// to hook stdout and stderr.
        /// The hooks are called 'stdoutHook' and 'stderrHook'.
        /// </summary>
        /// <returns>New expando object.</returns>
        public ExpandoObject MakeHookedExpando()
        {
            var expando = new ExpandoObject();
            HookExpando(expando);
            return expando;
        }

        /// <summary>
        /// Adds the standard stdout and stderr hooks to the specified expando.
        /// The hooks are called 'stdoutHook' and 'stderrHook'.
        /// </summary>
        /// <param name="expando">The expando to add the hooks to.</param>
        public void HookExpando(ExpandoObject expando)
        {
            expando.SetProperty("stdoutHook", GetStdoutHook());
            expando.SetProperty("stderrHook", GetStderrHook());
        }

        /// <summary>
        /// Returns an Edge-compatible Func that can be used to hook stdout.
        /// </summary>
        /// <returns>Delegate to be used to hook stdout.</returns>
        public Func<object, Task<object>> GetStdoutHook()
        {
            Func<object, Task<object>> hook = (message) =>
            {
                TheLogger.LogMessage((message as string).Trim());
                return Task.FromResult<object>(null);
            };

            return hook;
        }

        /// <summary>
        /// Returns an Edge-compatible Func that can be used to hook stderr.
        /// </summary>
        /// <returns>Delegate to be used to hook stderr.</returns>
        public Func<object, Task<object>> GetStderrHook()
        {
            Func<object, Task<object>> hook = (message) =>
            {
                TheLogger.LogError((message as string).Trim());
                return Task.FromResult<object>(null);
            };

            return hook;
        }

        //public IEnumerable<NodeMessage> UnpackMessages(dynamic messages)
        //{
        //    var msgs = new List<NodeMessage>();

        //    if (messages != null)
        //    { 
        //        foreach (var message in messages)
        //        {
        //            var msg = new NodeMessage(message.message, message.stream[0] == 'e');
        //            msgs.Add(msg);
        //        }
        //    }

        //    return msgs;
        //}





        





        //public Func<object, Task<object>> GetStdoutHook2()
        //{
        //    Func<object, Task<object>> hook = (message) =>
        //    {
        //        Task<object> t = Task.Factory.StartNew<object>(() => {
        //                TheLogger.LogMessage((message as string).Trim());
        //                return null;
        //            });

        //        return t;
        //    };

        //    return hook;
        //}

        //public Func<object, Task<object>> GetStdoutHook2()
        //{
        //    Func<object, Task<object>> hook = (message) =>
        //    {
        //        Task<object> t = Task.Factory.StartNew<object>(() =>
        //        {
        //            TheLogger.LogMessage((message as string).Trim());
        //            return null;
        //        });

        //        return t;
        //    };

        //    return hook;
        //}

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
