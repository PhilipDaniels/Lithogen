﻿using System;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.ExamplePlugin
{
    /// <summary>
    /// An example file processor. Drop into the Plugins folder and see if it is loaded.
    /// </summary>
    public class SecondHtmlProcessor : IProcessor
    {
        const string LOG_PREFIX = "SecondHtmlProcessor: ";
        readonly ILogger TheLogger;

        public SecondHtmlProcessor(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Process(IPipelineFile file)
        {
            file.ThrowIfNull("file");

            TheLogger.LogMessage(LOG_PREFIX + "Timestamping {0}.", file.WorkingFileName);
            file.Contents += String.Format("{0}<!-- SecondHtmlProcessor {1} -->{2}", Environment.NewLine, DateTime.Now, Environment.NewLine);
        }
    }
}
