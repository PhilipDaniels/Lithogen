using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using BassUtils;
using Lithogen.Engine;
using Unosquare.Labs.EmbedIO.Modules;

namespace Lithogen
{
    public sealed class LiveReloadServer : WebSocketsServer, IDisposable
    {
        public string Directory { get; private set; }

        readonly DirectoryWatcher Watcher;
        readonly object WatcherPadlock;
        bool Disposed;

        public LiveReloadServer()
            : base(true, 0)
        {
            Directory = Program.TheSettings.LithogenWebsiteDirectory;
            Watcher = new DirectoryWatcher(Directory);
            WatcherPadlock = new object();
            Watcher.ChangedFiles += Watcher_ChangedFiles;
        }

        //public LiveReloadServer(string directory, WebSocket webSocket)
        //{
        //    Directory = directory.ThrowIfDirectoryDoesNotExist("directory");
        //    WebSocket = webSocket.ThrowIfNull("webSocket");
        //    Watcher = new DirectoryWatcher(Directory);
        //    WatcherPadlock = new object();
        //    Watcher.ChangedFiles += Watcher_ChangedFiles;
        //}

        /// <summary>
        /// Starts the server. It begins watching the <code>Directory</code>
        /// and will respond to websocket requests from the LiveReload client.
        /// </summary>
        //public void Start()
        //{
        //    Watcher.Start();
        //    EmitHelloMessage();
        //}

        /// <summary>
        /// Returns the JSON that must be sent to the client during the handshake process.
        /// </summary>
        /// <returns></returns>
        public static string HelloJson
        {
            get
            {
                return
@"{
""command"": ""hello"",
""protocols"": [""http://livereload.com/protocols/official-7""],
""serverName"": ""LithogenLiveReloadServer""
}";
            }
        }

        public IEnumerable<string> GetReloadJson(IEnumerable<string> files)
        {
            files.ThrowIfNull("files");

            foreach (string file in files)
                yield return GetReloadJson(file);
        }

        public string GetReloadJson(string file)
        {
            file.ThrowIfNullOrWhiteSpace("file");

            if (file.StartsWith(Directory))
                file = file.Substring(Directory.Length + 1);

            file = file.Replace('\\', '/');

            string template =
@"{
""command"": ""reload"",
""path"": ""THEPATH"",
""liveCSS"": ""true""
}";
            return template.Replace("THEPATH", file);
        }

        public new void Dispose()
        {
            if (!Disposed)
            {
                Watcher.Dispose();
                base.Dispose();
                Disposed = true;
            }
        }

        void Watcher_ChangedFiles(object sender, DirectoryWatcherEventArgs e)
        {
            lock (WatcherPadlock)
            {
                // Alas, we get events for directories (which we don't care about).
                var filtered = (from n in e.FileSystemEvents
                                where File.Exists(n.FullPath)
                                select n.FullPath
                                ).Distinct();

                if (filtered.Any())
                {
                    foreach (var file in filtered)
                        EmitReloadMessage(file);
                }
            }
        }

        void EmitHelloMessage()
        {
            //var buffer = System.Text.Encoding.UTF8.GetBytes(HelloJson);
            Send(this.WebSockets[0], HelloJson);
            //await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        void EmitReloadMessage(string file)
        {
            Console.WriteLine("Sending reload message for {0}", file);
            string json = GetReloadJson(file);
            Send(this.WebSockets[0], json);

            //var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            //await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        #region EmbedIO support
        /// <summary>
        /// Called when this WebSockets Server receives a full message (EndOfMessage) form a WebSockets client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rxBuffer">The rx buffer.</param>
        /// <param name="rxResult">The rx result.</param>
        protected override void OnMessageReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
        {
            var msg = Encoding.UTF8.GetString(rxBuffer);
            Console.WriteLine("WS OnMessageReceived: " + msg);
        }

        public override string ServerName
        {
            get { return "Lithogen LiveReload Server"; }
        }


        /// <summary>
        /// Called when this WebSockets Server accepts a new WebSockets client.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnClientConnected(WebSocketContext context)
        {
            Console.WriteLine("WS: OnClientConnected");
            EmitHelloMessage();
            Console.WriteLine("WS: OnClientConnected - hello sent, starting watcher.");
            Watcher.Start();
        }

        /// <summary>
        /// Called when this WebSockets Server receives a message frame regardless if the frame represents the EndOfMessage.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rxBuffer">The rx buffer.</param>
        /// <param name="rxResult">The rx result.</param>
        protected override void OnFrameReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
        {
            return;
        }

        /// <summary>
        /// Called when the server has removed a WebSockets connected client for any reason.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnClientDisconnected(WebSocketContext context)
        {
            Console.WriteLine("WS: OnClientDisconnected");
        }
        #endregion
    }
}
