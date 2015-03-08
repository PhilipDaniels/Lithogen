using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Lithogen.DI
{
    class SettingsConvention : IParameterConvention
    {
        //const string ConnectionStringPostFix = "ConnectionString";

        [DebuggerStepThrough]
        public bool CanResolve(ParameterInfo parameter)
        {
            bool resolvable = parameter.ParameterType == typeof(string) &&
                              parameter.Name == "viewsDirectory";
            //parameter.Name.EndsWith(ConnectionStringPostFix) &&
            //parameter.Name.LastIndexOf(ConnectionStringPostFix) > 0;

            //if (resolvable)
            //    this.VerifyConfigurationFile(parameter);

            return resolvable;
        }

        [DebuggerStepThrough]
        public Expression BuildExpression(ParameterInfo parameter)
        {
            //var constr = this.GetConnectionString(parameter);

            return Expression.Constant("MyViewsDir", typeof(string));
        }

        //[DebuggerStepThrough]
        //private void VerifyConfigurationFile(ParameterInfo parameter)
        //{
        //    this.GetConnectionString(parameter);
        //}

        //[DebuggerStepThrough]
        //private string GetConnectionString(ParameterInfo parameter)
        //{
        //    string name = parameter.Name.Substring(0,
        //        parameter.Name.LastIndexOf(ConnectionStringPostFix));

        //    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

        //    if (settings == null)
        //    {
        //        throw new ActivationException(
        //            "No connection string with name '" + name +
        //            "' could be found in the application's " +
        //            "configuration file.");
        //    }

        //    return settings.ConnectionString;
        //}
    }
}
