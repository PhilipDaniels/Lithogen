using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class ViewPipeline : IViewPipeline
    {
        class Payload
        {
            public string Filename { get; set; }
            public bool IsMappedExtension { get; set; }
            public Exception Exception { get; set; }
            public IPipelineFile PipelineFile { get; set; }
        }

        const string LOG_PREFIX = "ViewPipeline: ";
        readonly ILogger TheLogger;
        readonly int ViewDOP;
        readonly IViewFileProvider ViewFileProvider;
        readonly IPartialCache PartialCache;
        readonly ICompositeModelInjector CompositeModelInjector;
        readonly IProcessorFactory ProcessorFactory;
        readonly IOutputFileWriter OutputFileWriter;
        readonly IPipelineFileLoader PipelineFileLoader;
        readonly IConfigurationResolver ConfigurationResolver;
        readonly IRebaser Rebaser;

        readonly TransformManyBlock<string, Payload> ViewDirectoryReaderBlock;
        readonly ActionBlock<Payload> ViewDirectoryReaderErrorBlock;
        readonly ActionBlock<Payload> EarlyCopyFileBlock;

        readonly TransformBlock<Payload, Payload> FileLoadBlock;
        readonly ActionBlock<Payload> FileLoadErrorBlock;

        readonly TransformBlock<Payload, Payload> ModelInjectionBlock;
        readonly ActionBlock<Payload> ModelInjectionErrorBlock;

        readonly TransformBlock<Payload, Payload> ApplyProcessorsBlock;
        readonly ActionBlock<Payload> ApplyProcessorsErrorBlock;

        readonly ActionBlock<Payload> OutputFileWriterBlock;
        readonly ActionBlock<Payload> DoNotPublishBlock;

        public ViewPipeline
            (
            ILogger logger,
            ISettings settings,
            IViewFileProvider viewFileProvider,
            IPartialCache partialCache,
            ICompositeModelInjector compositeModelInjector,
            IProcessorFactory processorFactory,
            IOutputFileWriter outputFileWriter,
            IPipelineFileLoader pipelineFileLoader,
            IConfigurationResolver configurationResolver,
            IRebaser rebaser
            )
        {
            TheLogger = logger.ThrowIfNull("logger");
            ViewDOP = settings.ThrowIfNull("settings").ViewDOP;
            ViewFileProvider = viewFileProvider.ThrowIfNull("viewFileProvider");
            PartialCache = partialCache.ThrowIfNull("partialCache");
            CompositeModelInjector = compositeModelInjector.ThrowIfNull("compositeModelInjector");
            ProcessorFactory = processorFactory.ThrowIfNull("processorFactory");
            OutputFileWriter = outputFileWriter.ThrowIfNull("outputFileWriter");
            PipelineFileLoader = pipelineFileLoader.ThrowIfNull("pipelineFileLoader");
            ConfigurationResolver = configurationResolver.ThrowIfNull("configurationResolver");
            Rebaser = rebaser.ThrowIfNull("rebaser");

            // Create the TPF Dataflow pipeline.
            // This block returns all the files in a directory that we need to process.
            ViewDirectoryReaderBlock = new TransformManyBlock<string, Payload>((filenameOrDirectory) =>
            {
                try
                {
                    if (Directory.Exists(filenameOrDirectory))
                    {
                        return from filename in ViewFileProvider.GetViewFiles(filenameOrDirectory)
                               select new Payload() { Filename = filename, IsMappedExtension = ConfigurationResolver.IsMappedExtension(filename) };
                    }
                    else if (File.Exists(filenameOrDirectory))
                    {
                        var payload = new Payload();
                        payload.Filename = filenameOrDirectory;
                        payload.IsMappedExtension = ConfigurationResolver.IsMappedExtension(filenameOrDirectory);
                        return Enumerable.Repeat(payload, 1);
                    }
                    else
                    {
                        return Enumerable.Empty<Payload>();
                    }
                }
                catch (Exception ex)
                {
                    var payload = new Payload();
                    payload.Filename = filenameOrDirectory;
                    payload.Exception = ex;
                    return Enumerable.Repeat(payload, 1);
                }
            });

            ViewDirectoryReaderErrorBlock = new ActionBlock<Payload>((file) =>
                {
                    LogException(file, "Error reading file or folder");
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            EarlyCopyFileBlock = new ActionBlock<Payload>((file) =>
                {
                    // This is a terminal block. We can handle our own errors.
                    try
                    {
                        OutputFileWriter.CopyFile(file.Filename);
                    }
                    catch (Exception ex)
                    {
                        file.Exception = ex;
                        LogException(file, "Error copying file");
                    }
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            FileLoadBlock = new TransformBlock<Payload, Payload>((file) =>
                {
                    try
                    {
                        file.PipelineFile = PipelineFileLoader.Load(file.Filename);
                    }
                    catch (Exception ex)
                    {
                        file.Exception = ex;
                    }

                    return file;
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            FileLoadErrorBlock = new ActionBlock<Payload>((file) =>
                {
                    LogException(file, "Error loading file");
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            ModelInjectionBlock = new TransformBlock<Payload, Payload>((file) =>
                {
                    try
                    {
                        CompositeModelInjector.InjectModels(file.PipelineFile);
                    }
                    catch (Exception ex)
                    {
                        file.Exception = ex;
                    }

                    return file;
                },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            ModelInjectionErrorBlock = new ActionBlock<Payload>((file) =>
                {
                    LogException(file, "Error injecting models");
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            ApplyProcessorsBlock = new TransformBlock<Payload, Payload>((file) =>
                {
                    try
                    {
                        ApplyProcessors(file.PipelineFile);
                    }
                    catch (Exception ex)
                    {
                        file.Exception = ex;
                    }

                    return file;
                },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = ViewDOP }
                );

            ApplyProcessorsErrorBlock = new ActionBlock<Payload>((file) =>
                {
                    LogException(file, "Error processing file");
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );


            OutputFileWriterBlock = new ActionBlock<Payload>((file) =>
                {
                    // This is a terminal block, we can handle our own errors.
                    try
                    {

                        OutputFileWriter.WriteFile(file.PipelineFile.WorkingFileName, file.PipelineFile.Contents);
                    }
                    catch (Exception ex)
                    {
                        file.Exception = ex;
                        LogException(file, "Error while writing output file");
                    }
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            // Should we run posts through the pipeline if Publish is false?...Can argue both ways.
            // Yes -> we will find any errors in the file, but maybe do extra work.
            DoNotPublishBlock = new ActionBlock<Payload>((file) =>
                {
                    try
                    {
                        TheLogger.LogMessage(LOG_PREFIX + "Publish is false for {0}, ignoring file.", file.Filename);
                    }
                    catch
                    {
                    }
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded }
                );

            // Link the blocks. Every block has its own error block so that we can propagate completion. The pipeline can
            // hang if you try to use the same error block in multiple places because of early completion propagation.
            ViewDirectoryReaderBlock.LinkTo(ViewDirectoryReaderErrorBlock, new DataflowLinkOptions { PropagateCompletion = true }, payload => payload.Exception != null);
            ViewDirectoryReaderBlock.LinkTo(EarlyCopyFileBlock, new DataflowLinkOptions { PropagateCompletion = true }, payload => !payload.IsMappedExtension);
            ViewDirectoryReaderBlock.LinkTo(FileLoadBlock, new DataflowLinkOptions { PropagateCompletion = true });

            FileLoadBlock.LinkTo(FileLoadErrorBlock, new DataflowLinkOptions { PropagateCompletion = true }, payload => payload.Exception != null);
            FileLoadBlock.LinkTo(ModelInjectionBlock, new DataflowLinkOptions { PropagateCompletion = true });

            ModelInjectionBlock.LinkTo(ModelInjectionErrorBlock, new DataflowLinkOptions { PropagateCompletion = true }, payload => payload.Exception != null);
            ModelInjectionBlock.LinkTo(ApplyProcessorsBlock, new DataflowLinkOptions { PropagateCompletion = true });

            ApplyProcessorsBlock.LinkTo(ApplyProcessorsErrorBlock, new DataflowLinkOptions { PropagateCompletion = true }, payload => payload.Exception != null);
            ApplyProcessorsBlock.LinkTo(OutputFileWriterBlock, new DataflowLinkOptions { PropagateCompletion = true }, file => file.PipelineFile.Publish);
            ApplyProcessorsBlock.LinkTo(DoNotPublishBlock, new DataflowLinkOptions { PropagateCompletion = true }, file => !file.PipelineFile.Publish);
        }

        public void ProcessFile(string fileName)
        {
            // It is ok for the file to not exist, we ignore it.
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            PostAndWait(fileName);
        }

        public void ProcessDirectory(string viewsDirectory)
        {
            viewsDirectory.ThrowIfDirectoryDoesNotExist("viewsDirectory");
            
            PostAndWait(viewsDirectory);
        }

        void PostAndWait(string filenameOrDirectory)
        {
            // We should never get any exceptions, each stage in the pipeline catches exceptions.
            try
            {
                PartialCache.Load();

                ViewDirectoryReaderBlock.Post(filenameOrDirectory);
                ViewDirectoryReaderBlock.Complete();

                // Wait for all terminal blocks to complete.
                Task.WhenAll
                    (
                    ViewDirectoryReaderErrorBlock.Completion,
                    EarlyCopyFileBlock.Completion,
                    FileLoadErrorBlock.Completion,
                    ModelInjectionErrorBlock.Completion,
                    ApplyProcessorsErrorBlock.Completion,
                    OutputFileWriterBlock.Completion,
                    DoNotPublishBlock.Completion
                    ).Wait();
            }
            catch (AggregateException aex)
            {
                TheLogger.LogError(LOG_PREFIX + "Pipeline Aggregate error: {0}", aex.Message);
            }
            catch (Exception ex)
            {
                TheLogger.LogError(LOG_PREFIX + "Pipeline error: {0}", ex.Message);
            }
        }

        void ApplyProcessors(IPipelineFile file)
        {
            file.ThrowIfNull("file");

            TheLogger.LogMessage("ApplyProcessors: Processing {0}", file.FileName);

            while (true)
            {
                // Apply all processors for the current extension. We will keep doing this until
                // we meet an extension we don't recognise.
                // Foo.cshtml -> Foo.html (2 processors)
                string currentExtension = FileUtils.GetCleanExtension(file.WorkingFileName);
                var processors = ProcessorFactory.MakeProcessors(file.GetProcessorNames(currentExtension));
                foreach (var processor in processors)
                    processor.Process(file);

                string newExtension = FileUtils.GetCleanExtension(file.WorkingFileName);
                if (newExtension.Equals(currentExtension, System.StringComparison.OrdinalIgnoreCase))
                {
                    // Extension did not change. Typical behaviour for a html -> html filter.
                    string pe = FileUtils.GetPenultimateExtension(file.WorkingFileName);
                    if (file.DefaultConfiguration.ExtensionMappings.ContainsKey(pe))
                    {
                        // Just strip this current extension, the next extension will then be used
                        // the next time around the loop. Example: "Foo.md.html" becomes "Foo.md".
                        file.WorkingFileName = Path.GetFileNameWithoutExtension(file.WorkingFileName);
                    }
                    else
                    {
                        // We have no processor that can handle this file now. Just terminate.
                        break;
                    }
                }
                else
                {
                    // Extension did change. Typical behaviour for a cshtml -> html processor.
                    // We go round again and pick up a new set of processors.
                }
            }

            // Replace the special marker PATHTOROOT(~) with a relative path.
            file.Contents = Rebaser.ReplaceRootsInFile(file.FileName, file.Contents);
        }

        void LogException(Payload payload, string leader)
        {
            TheLogger.LogError(LOG_PREFIX + leader + ": " + payload.Filename + "; " + payload.Exception.Message);
        }
    }
}
