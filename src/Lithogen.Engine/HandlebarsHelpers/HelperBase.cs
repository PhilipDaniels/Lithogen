using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Handlebars;

/*
 * There are two types of helpers in Handlebars.
 * 
 * 1. Expression helpers (type = HandlebarsHelper)
 * Given an argument(s), these produce output. In the JavaScript world they typically return a string,
 * but in Handlebars.Net they have a return type of void and work by writing a string to the output stream.
 *     Signature: delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
 *     Example:  {{now "yyyy-mm-dd"}
 *       (defines an expression helper called "now" that takes one parameter, a text string "yyyy-mm-dd").
 *       
 * 
 * 2. Block helpers.
 * These receive, in addition, a pair of templates, the first template being for the "logically true" part,
 * and the second for the "logically negative" part. 
 *     Signature: delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments); 
 *     Example:
 
           {{#ifCond "3" "3"}}
               <p>3 and 3 are equal</p>
           {{else}}
               <p>2 and 3 are not equal</p>
           {{/ifCond}}
 * 
 * Here "<p>3 and 3 are equal</p>" is passed as options.Template and "<p>2 and 3 are not equal</p>" is passed
 * as options.Inverse.
 * 
 * See BuiltinHelpers.cs in the Handlebars.Net source code for some examples.
 */

namespace Lithogen.Engine.HandlebarsHelpers
{
    public abstract class HelperBase
    {
        public IEnumerable<KeyValuePair<string, HandlebarsHelper>> Helpers
        {
            get
            {
                return GetHelpers<HandlebarsHelper>();
            }
        }

        public IEnumerable<KeyValuePair<string, HandlebarsBlockHelper>> BlockHelpers
        {
            get
            {
                return GetHelpers<HandlebarsBlockHelper>();
            }
        }

        IEnumerable<KeyValuePair<string, T>> GetHelpers<T>()
        {
            var helpersType = this.GetType();

            foreach (var method in helpersType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
            {
                var possibleDelegate = Delegate.CreateDelegate(typeof(T), method, false);
                if (possibleDelegate != null)
                {
                    yield return new KeyValuePair<string, T>
                        (
                        //((DescriptionAttribute)Attribute.GetCustomAttribute(method, typeof(DescriptionAttribute))).Description,
                        method.Name,
                        (T)(object)possibleDelegate
                        );
                }
            }
        }

        protected static T RequireOneArgument<T>(string helperName, params object[] arguments)
        {
            if (arguments.Length != 1)
                throw new HandlebarsException("{{" + helperName + "}} helper must have exactly one argument");

            T arg1 = (T)Convert.ChangeType(arguments[0], typeof(T));
            return arg1;
        }

        protected static Tuple<T1, T2> RequireTwoArguments<T1, T2>(string helperName, params object[] arguments)
        {
            if (arguments.Length != 2)
                throw new HandlebarsException("{{" + helperName + "}} helper must have exactly two arguments");

            T1 arg1 = (T1)Convert.ChangeType(arguments[0], typeof(T1));
            T2 arg2 = (T2)Convert.ChangeType(arguments[1], typeof(T2));

            var tuple = Tuple.Create<T1, T2>(arg1, arg2);
            return tuple;
        }

        protected static Tuple<T1, T2, T3> RequireThreeArguments<T1, T2, T3>(string helperName, params object[] arguments)
        {
            if (arguments.Length != 3)
                throw new HandlebarsException("{{" + helperName + "}} helper must have exactly three arguments");

            T1 arg1 = (T1)Convert.ChangeType(arguments[0], typeof(T1));
            T2 arg2 = (T2)Convert.ChangeType(arguments[1], typeof(T2));
            T3 arg3 = (T3)Convert.ChangeType(arguments[2], typeof(T3));

            var tuple = Tuple.Create<T1, T2, T3>(arg1, arg2, arg3);
            return tuple;
        }
    }
}
