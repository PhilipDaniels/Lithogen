using SimpleInjector;
using System;
using System.Linq;

namespace Lithogen.DI
{
    static class ContainerExtensions
    {
        public static bool IsRegisteredAsSingleton(this Container container, Type interfaceType)
        {
            var p = container.GetCurrentRegistrations().Where(producer => producer.ServiceType == interfaceType).FirstOrDefault();
            if (p == null)
                return false;
            else
                return p.Lifestyle == Lifestyle.Singleton;
        }
    }
}
