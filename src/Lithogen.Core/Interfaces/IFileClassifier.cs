using Lithogen.Core;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Classifies files within the project directory into their various types.
    /// </summary>
    public interface IFileClassifier
    {
        /// <summary>
        /// Given a filename, work out where in the project structure it lives
        /// and determine its <code>FileClass</code>.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <returns>The FileClass of the filename.</returns>
        FileClass Classify(string filename);
    }
}
