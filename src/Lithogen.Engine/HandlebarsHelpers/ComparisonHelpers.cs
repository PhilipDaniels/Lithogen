namespace Lithogen.Engine.HandlebarsHelpers
{
    public class ComparisonHelpers
    {
        /*
         * {{#ifCond var1 '==' var2}}
         *     http://stackoverflow.com/questions/8853396/logical-operator-in-a-handlebars-js-if-conditional?rq=1
         * {{#ifeq}}    
         * {{#ifneq}}    
         * 
         * 
         *     "contains"
         * {{#str "contains" "startsWith", "toLower", "dashify" etc.}}    
         * {{#fmt "fmtstring", "thing"}} - call String.Format(). What culture?
         * {{#date "fmtstring "Now"}}
         * {{#moment "fmtstring" date}}
         * {{#any}}
         * {{#all}} (replaces and)
         * {{#inflect / #ordinalize}}
         * {{ceil, floor, round, sum, min, avg, max, add, sub, multiply, divide}}
         */

        /*
        public static void and(TextWriter writer, HelperOptions options, dynamic context, params object[] arguments)
        {
            var args = RequireTwoArguments<object, object>("and", arguments);

            if (Handlebars.HandlebarsUtils.IsTruthy(args.Item1) && Handlebars.HandlebarsUtils.IsTruthy(args.Item2))
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        }
        */
    }




    /*
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
*/

}
