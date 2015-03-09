using System;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandLine
{
    /// <summary>
    /// The set of standard build steps. Used to match and normalize command line arguments
    /// and to generate the build commands.
    /// </summary>
    public class StandardBuildSteps : IStandardBuildSteps
    {
        public string ContentStepName { get { return "content"; } }
        public string ScriptsStepName { get { return "scripts"; } }
        public string ImagesStepName { get { return "images"; } }
        public string ViewsStepName { get { return "views"; } } 

        public string GetMatch(string possible)
        {
            if (MatchNpm(possible))
                return possible;
            else if (MatchNode(possible))
                return possible;
            else if (MatchContent(possible))
                return ContentStepName;
            else if (MatchScripts(possible))
                return ScriptsStepName;
            else if (MatchImages(possible))
                return ImagesStepName;
            else if (MatchViews(possible))
                return ViewsStepName;
            else
                return null;
        }

        public bool MatchNpm(string possible)
        {
            return possible.StartsWith("npm ", StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchNode(string possible)
        {
            return possible.StartsWith("node ", StringComparison.OrdinalIgnoreCase);
        }
        
        public bool MatchContent(string possible)
        {
            return Match(possible, ContentStepName);
        }

        public bool MatchScripts(string possible)
        {
            return Match(possible, ScriptsStepName);
        }

        public bool MatchImages(string possible)
        {
            return Match(possible, ImagesStepName);
        }

        public bool MatchViews(string possible)
        {
            return Match(possible, ViewsStepName);
        }
        
        bool Match(string possible, string step)
        {
            possible = possible.ToLowerInvariant();
            return possible == step || possible == step[0].ToString();
        }
    }
}
