using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Lithogen.Core.Interfaces
{
    public interface IEdgeHelper
    {
        /// <summary>
        /// Replace the special token that indicates the folder where the node installation
        /// is for a particular project.
        /// </summary>
        /// <param name="javascript">The javascript to do the replacement in.</param>
        /// <returns>New javascript.</returns>
        string ReplaceLithogenNodeRoot(string javascript);

        /// <summary>
        /// Creates a new ExpandoObject that contains two properties designed
        /// to hook stdout and stderr.
        /// The hooks are called 'stdoutHook' and 'stderrHook'.
        /// </summary>
        /// <returns>New expando object.</returns>
        ExpandoObject MakeHookedExpando();

        /// <summary>
        /// Adds the standard stdout and stderr hooks to the specified expando.
        /// The hooks are called 'stdoutHook' and 'stderrHook'.
        /// </summary>
        /// <param name="expando">The expando to add the hooks to.</param>
        void HookExpando(ExpandoObject expando);

        /// <summary>
        /// Returns an Edge-compatible Func that can be used to hook stdout.
        /// </summary>
        /// <returns>Delegate to be used to hook stdout.</returns>
        Func<object, Task<object>> GetStdoutHook();

        /// <summary>
        /// Returns an Edge-compatible Func that can be used to hook stderr.
        /// </summary>
        /// <returns>Delegate to be used to hook stderr.</returns>
        Func<object, Task<object>> GetStderrHook();
    }
}
