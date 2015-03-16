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
