namespace Lithogen.Core.Interfaces
{
    public interface IPipelineFileLoader
    {
        /// <summary>
        /// Loads the given file, constructing a pipeline file object
        /// with relevant context.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>Loaded file.</returns>
        IPipelineFile Load(string filename);
    }
}
