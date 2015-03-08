namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Model injectors are responsible for loading data such as found in
    /// Json or Yaml files and making them available to the file being processed.
    /// </summary>
    public interface IModelInjector
    {
        /// <summary>
        /// Find any available data and add it to the file.
        /// </summary>
        /// <param name="file">The pipeline file.</param>
        void Inject(IPipelineFile file);
    }
}
