using SimpleInjector.Advanced;
using System.Linq.Expressions;
using System.Reflection;

namespace Lithogen.DI
{
    /*
    class ConventionConstructorInjectionBehavior : IConstructorInjectionBehavior
    {
        readonly IConstructorInjectionBehavior Decorated;
        readonly IParameterConvention Convention;

        public ConventionConstructorInjectionBehavior(IConstructorInjectionBehavior decorated, IParameterConvention convention)
        {
            Decorated = decorated;
            Convention = convention;
        }

        public Expression BuildParameterExpression(ParameterInfo parameter)
        {
            if (!Convention.CanResolve(parameter))
                return Decorated.BuildParameterExpression(parameter);

            return Convention.BuildExpression(parameter);
        }
    }
    */
}
