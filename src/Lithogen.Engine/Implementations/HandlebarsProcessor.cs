using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using BassUtils;
using EdgeJs;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

/*
 * In _Layout.hbs:
 *   {{>body}}
 *   {{>otherPartials}}
 * 
 * 
 * In Template.hbs:
 * ---
 * layout: _Layout.hbs
 * ---
 *   <h1>my test</h1>
 *   {{>somethingelse}}
 * 
 * 
 * In somethingelse.hbs:
 * <div>Some text</div>
 */

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// The default processor for Handlebars templates.
    /// </summary>
    public class HandlebarsProcessor : IProcessor
    {
        const string LOG_PREFIX = "HandlebarsProcessor: ";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        readonly IModelFactory ModelFactory;
        readonly IPartialCache PartialCache;
        readonly string[] HandleBarsExtensions;

        public HandlebarsProcessor
            (
            ILogger logger,
            ISettings settings,
            IModelFactory modelFactory,
            IPartialCache partialCache
            )
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
            ModelFactory = modelFactory.ThrowIfNull("modelFactory");
            PartialCache = partialCache.ThrowIfNull("partialCache");
            HandleBarsExtensions = new string[] { "hbs", "hb", "handlebars" };
        }

        public void Process(IPipelineFile file)
        {
            try
            {
                file.ThrowIfNull("file");
                file.Contents.ThrowIfNull("file.Contents");

                TheLogger.LogVerbose(LOG_PREFIX + "Compiling {0}.", file.FileName);
                ProcessImpl(file);
            }
            catch
            {
                TheLogger.LogError(LOG_PREFIX + "Could not compile {0}", file.FileName);
                throw;
            }
        }

        void ProcessImpl(IPipelineFile file)
        {
            // But what about context? What is "this" passed to each partial?
            // TODO: Just register all partials in the partial cache?
            // TODO: Allow layouts in layouts.
            // TODO: Allow us to run anything through handlebars without changing the file extension.

            try
            {
                var func = Edge.Func(@"
                    var hbars = require('./../handlebars');

                    return function(payload, callback) {
                        for (key in payload.partials) {
                            hbars.registerPartial(key, payload.partials[key]);
                        }

                        var template = hbars.compile(payload.source);
                        var templateResult = template(payload.context);
                        callback(null, templateResult);
                    }");


                var partials = new Dictionary<string, string>();

                ITextFile toCompile = null;
                if (file.Layout != null)
                {
                    // We expect the layouts to be in the partial cache already.
                    var layout = PartialCache.ResolvePartial(file.Layout, null);
                    if (layout == null)
                    {
                        TheLogger.LogError(LOG_PREFIX + "Could not locate layout {0} for file {1}", file.Layout, file.FileName);
                    }
                    else
                    {
                        AddPartials(partials, file);
                        AddPartials(partials, layout);
                        partials["body"] = file.Contents;
                        toCompile = layout;
                    }
                }
                else
                {
                    AddPartials(partials, file);
                    toCompile = file;
                }
                
                dynamic payload = new ExpandoObject();
                payload.partials = partials;
                payload.source = toCompile.Contents;
                payload.context = MakeViewBag(file);

                file.Contents = func(payload).Result.ToString();
            }
            catch (Exception ex)
            {
            }
            
            string newExtension = file.ExtOut ?? "html";
            file.WorkingFileName = Path.ChangeExtension(file.WorkingFileName, newExtension);           
        }

        void AddPartials(Dictionary<string, string> partials, ITextFile file)
        {
            var partialNames = GetPartialNames(file.Contents);

            // Try and find the partial and register it.
            string dir = Path.GetDirectoryName(file.FileName); 
            foreach (string partialName in partialNames)
            {
                if (Path.HasExtension(partialName))
                {
                    string filename = Path.Combine(dir, partialName);
                    AddPartials(partials, filename);
                }
                else
                {
                    foreach (string ext in HandleBarsExtensions)
                    {
                        string filename = Path.Combine(dir, partialName + "." + ext);
                        AddPartials(partials, filename);
                    }
                }
            }
        }

        void AddPartials(Dictionary<string, string> partials, string filename)
        {
            if (!File.Exists(filename))
                return;

            string partialName = Path.GetFileNameWithoutExtension(filename);
            var f = new TextFile(filename);
            partials[partialName] = f.Contents;
            AddPartials(partials, f);
        }

        /// <summary>
        /// Get all partials in a template.
        /// "Body" is special, and is used to indicate the file that is
        /// being included in a layout, and hence is excluded from this list.
        /// </summary>
        static IEnumerable<string> GetPartialNames(string contents)
        {
            var names = new List<string>();

            string pattern = @".*\{\{\>\s*(?<pname>\w*)\s*\}\}";
            var match = Regex.Match(contents, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            while (match.Success)
            {
                string name = match.Groups["pname"].Value;
                if (!name.Equals("body", StringComparison.OrdinalIgnoreCase))
                    names.Add(name);
                match = match.NextMatch();
            }
            
            return names;
        }

        ExpandoObject MakeViewBag(IPipelineFile file)
        {
            var vb = new ExpandoObject();
            if (file.ModelName != null)
            {
                Type modelType = ModelFactory.GetModelType(file.ModelName);
                if (modelType != null)
                {
                    object model = ModelFactory.CreateModelInstance(modelType);
                    file.Data.SetProperty("model", model);
                }
                else
                {
                    TheLogger.LogError(LOG_PREFIX + "The model {0} could not be found in the model search directories.", file.ModelName);
                }
            }

            vb.Merge(file.Data);
            vb.SetProperty("UserData", file.UserData);
            vb.SetProperty("FileInfo", file.FileInfo);
            vb.SetProperty("Settings", TheSettings);
            return vb;
        }


        //void RegisterHelpers()
        //{
        //    var c = new ComparisonHelpers();

        //    foreach (var h in c.BlockHelpers)
        //        Hbars.RegisterHelper(h.Key, h.Value);
        //    foreach (var h in c.Helpers)
        //        Hbars.RegisterHelper(h.Key, h.Value);
        //}
    }
}
