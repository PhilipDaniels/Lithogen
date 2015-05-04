namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Processors are responsible for transforming input files into output.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Processes the pipeline file, probably transforming its contents
        /// and often changing the working filename extension.
        /// </summary>
        /// <param name="file">The file to process.</param>
        void Process(IPipelineFile file);
    }
}
