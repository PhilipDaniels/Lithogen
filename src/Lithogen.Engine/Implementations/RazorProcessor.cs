using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Lithogen.Core;
using Lithogen.Core.Interfaces;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// The default processor for Razor templates. Encodes as HTML.
    /// </summary>
    public class RazorProcessor : IProcessor
    {
        const string LOG_PREFIX = "RazorProcessor: ";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        readonly IModelFactory ModelFactory;
        readonly IPartialCache PartialCache;

        public RazorProcessor
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
        }

        /// <summary>
        /// Processes the pipeline file, probably transforming its contents.
        /// </summary>
        /// <param name="file">The file to process.</param>
        public void Process(IPipelineFile file)
        {
            try
            {
                file.ThrowIfNull("file");
                file.Contents.ThrowIfNull("file.Contents");

                TheLogger.LogVerbose(LOG_PREFIX + "Compiling {0}", file.FileName);

                // Create a new instance of the RazorEngine.
                var config = new TemplateServiceConfiguration();
                config.Language = GetLanguage(file.FileName);
                config.TemplateManager = new RazorTemplateManager(PartialCache, file);
                using (var razorEngineService = RazorEngineService.Create(config))
                {

                    // Tell it about all the partials.
                    foreach (var p in PartialCache.Files.Where(p => IsRazorFile(p.FileName)))
                        razorEngineService.AddTemplate(p.FileName, p.Contents);

                    dynamic vb = MakeViewBag(file);

                    // Inject the default layout if there is not one in the file.
                    string layoutName = GetLayoutNameFromFile(file.Contents);
                    if (layoutName == null && file.Layout != null)
                        file.Contents = String.Format(CultureInfo.InvariantCulture, "@{{ Layout = @\"{0}\"; }}{1}{2}", file.Layout, Environment.NewLine, file.Contents);

                    // Inject the default model if there is not one in the file.
                    string modelTypeName = GetModelTypeNameFromFile(file.Contents);
                    if (modelTypeName == null && file.ModelName != null)
                    {
                        file.Contents = String.Format(CultureInfo.InvariantCulture, "@model = {0}{1}{2}", file.ModelName, Environment.NewLine, file.Contents);
                        modelTypeName = file.ModelName;
                    }

                    if (modelTypeName == null)
                    {
                        // There's no model - simple compile.
                        file.Contents = razorEngineService.RunCompile(file.Contents, file.FileName, null, null, (DynamicViewBag)vb);
                    }
                    else
                    {
                        Type modelType = ModelFactory.GetModelType(modelTypeName);
                        if (modelType != null)
                        {
                            object model = ModelFactory.CreateModelInstance(modelType);
                            //TheLogger.LogVerbose(LOG_PREFIX + "Retrieved instance of model {0}", modelTypeName);
                            file.Contents = razorEngineService.RunCompile(file.Contents, file.FileName, modelType, model, (DynamicViewBag)vb);
                        }
                        else
                        {
                            TheLogger.LogError(LOG_PREFIX + "The model {0} could not be found in the model search directories.", modelTypeName);
                            return;
                        }
                    }

                    string newExtension = file.ExtOut ?? "html";
                    file.WorkingFileName = Path.ChangeExtension(file.WorkingFileName, newExtension);
                }
            }
            catch
            {
                TheLogger.LogError(LOG_PREFIX + "Could not compile {0}", file.FileName);
                throw;
            }
        }

        /// <summary>
        /// Look for "@model My.Namespace.Type".
        /// </summary>
        /// <param name="contents">The contents to look in.</param>
        /// <returns>The typename, e.g. My.Namespace.Type.</returns>
        static string GetModelTypeNameFromFile(string contents)
        {
            string pattern = @"^\s*\@model\s+(?<typename>[^\s]*)\s*?$";
            var match = Regex.Match(contents, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups["typename"].Value;
            else
                return null;
        }

        /// <summary>
        /// Look for "@layout MyFile".
        /// </summary>
        /// <param name="contents">The contents to look in.</param>
        /// <returns>The layout name, e.g. _Layout.cshtml</returns>
        static string GetLayoutNameFromFile(string contents)
        {
            string pattern = @".*\@{.*\s*Layout\s*=\s*\@?""(?<layoutname>.*?)"";";

            var match = Regex.Match(contents, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups["layoutname"].Value;
            else
                return null;
        }

        static RazorEngine.Language GetLanguage(string fileName)
        {
            string ext = FileUtils.GetCleanExtension(fileName);
            if (ext.Equals("cshtml", StringComparison.OrdinalIgnoreCase))
                return RazorEngine.Language.CSharp;
            else if (ext.Equals("vbhtml", StringComparison.OrdinalIgnoreCase))
                return RazorEngine.Language.VisualBasic;
            else
                throw new ArgumentException("Unknown Razor template language.");
        }

        static bool IsRazorFile(string fileName)
        {
            string ext = FileUtils.GetCleanExtension(fileName);
            return ext.Equals("cshtml", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals("vbhtml", StringComparison.OrdinalIgnoreCase);
        }

        dynamic MakeViewBag(IPipelineFile file)
        {
            dynamic vb = new DynamicViewBag();
            MergeDynamicIntoRazorViewBag(vb, file.Data);
            vb.UserData = file.UserData;
            vb.FileInfo = file.FileInfo;
            vb.Settings = TheSettings;
            return vb;
        }

        static void MergeDynamicIntoRazorViewBag(DynamicViewBag dynamicViewBag, dynamic source)
        {
            var s = (IDictionary<string, object>)source;

            foreach (var kvp in s)
                dynamicViewBag.SetValue(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// RazorEngine calls this to get the body of templates.
        /// We need it to make partials work.
        /// See example at https://github.com/Antaris/RazorEngine
        /// and also https://antaris.github.io/RazorEngine/Caching.html
        /// </summary>
        class RazorTemplateManager : ITemplateManager
        {
            public IPartialCache PartialCache { get; private set; }
            public IPipelineFile File { get; private set; }

            public RazorTemplateManager(IPartialCache partialCache, IPipelineFile file)
            {
                PartialCache = partialCache.ThrowIfNull("partialCache");
                File = file.ThrowIfNull("file");
            }

            /// <summary>
            /// Resolve your template here (e.g. read from disk)
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public ITemplateSource Resolve(ITemplateKey key)
            {
                // This gets called only to get the body of the view file.
                if (key.TemplateType == ResolveType.Global)
                {
                    return new LoadedTemplateSource(File.Contents, File.FileName);
                }
                else if (key.TemplateType == ResolveType.Layout || key.TemplateType == ResolveType.Include)
                {
                    var partial = PartialCache.ResolvePartial(key.Name, null);
                    return new LoadedTemplateSource(partial.Contents, partial.FileName);
                }
                else
                {
                    throw new ArgumentException("Unknown TemplateType " + key.TemplateType);
                }
            }

            public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
            {
                // First time in (AddTemplate for the layout file) : name is the full filename, resolveType is Global and context is null.
                // Second time in (RunCompile for index.cshtml)    : name is the full filename, resolveType is Global and context is null.
                // Third time in                                   : with the same filename (index.cshtml)
                //     This call is so the engine can figure out the key to call Resolve() with.
                //     Then Resolve() gets called, with key.Name equal to index.cshtml
                // Fourth time in                                  : name is "shared\_Layout.cshtml", resolveType is Layout and context.Name is index.cshtml
                //     This call is asking for the key to find the layout body.
                //     Then Resolve() gets called, with context.Name equal to "shared\_Layout.cshtml"
                // at which point we have no way of returning that.


                // If you can have different templates with the same name depending on the 
                // context or the resolveType you need your own implementation here!
                // Otherwise you can just use NameOnlyTemplateKey.
                return new NameOnlyTemplateKey(name, resolveType, context);
            }

            public void AddDynamic(ITemplateKey key, ITemplateSource source)
            {
                // You can disable dynamic templates completely, but 
                // then all convenience methods (Compile and RunCompile) with
                // a TemplateSource will no longer work (they are not really needed anyway).
                //throw new NotImplementedException("dynamic templates are not supported!");
            }
        }
    }
}
