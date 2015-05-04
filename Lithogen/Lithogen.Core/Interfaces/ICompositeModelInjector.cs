namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// The ICompositeModelInjector represents a set of model injectors.
    /// </summary>
    public interface ICompositeModelInjector
    {
        /// <summary>
        /// Instruct the ICompositeModelInjector to tell each of its model
        /// injectors to operate.
        /// </summary>
        void InjectModels(IPipelineFile file);
    }
}
