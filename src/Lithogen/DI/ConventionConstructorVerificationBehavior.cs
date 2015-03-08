using SimpleInjector.Advanced;
using System.Reflection;

namespace Lithogen.DI
{
    class ConventionConstructorVerificationBehavior : IConstructorVerificationBehavior
    {
        readonly IConstructorVerificationBehavior Decorated;
        readonly IParameterConvention Convention;

        public ConventionConstructorVerificationBehavior(IConstructorVerificationBehavior decorated, IParameterConvention convention)
        {
            Decorated = decorated;
            Convention = convention;
        }

        public void Verify(ParameterInfo parameter)
        {
            if (!Convention.CanResolve(parameter))
                Decorated.Verify(parameter);
        }
    }
}
