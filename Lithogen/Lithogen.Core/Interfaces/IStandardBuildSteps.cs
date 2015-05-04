namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Represents logic around the standard build steps that Lithogen supports.
    /// </summary>
    public interface IStandardBuildSteps
    {
        string ContentStepName { get; }
        string ScriptsStepName { get; }
        string ImagesStepName { get; }
        string ViewsStepName { get; }

        string GetMatch(string possible);
        bool MatchNpm(string possible);
        bool MatchNode(string possible);
        bool MatchContent(string possible);
        bool MatchScripts(string possible);
        bool MatchImages(string possible);
        bool MatchViews(string possible);
    }
}
