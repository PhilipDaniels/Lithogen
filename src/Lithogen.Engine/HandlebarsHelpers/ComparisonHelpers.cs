using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handlebars;

namespace Lithogen.Engine.HandlebarsHelpers
{
    public class ComparisonHelpers : HelperBase
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

        public static void and(TextWriter writer, HelperOptions options, dynamic context, params object[] arguments)
        {
            var args = RequireTwoArguments<object, object>("and", arguments);

            if (Handlebars.HandlebarsUtils.IsTruthy(args.Item1) && Handlebars.HandlebarsUtils.IsTruthy(args.Item2))
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        }
    }
}
