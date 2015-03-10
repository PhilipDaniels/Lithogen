using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lithogen.Core
{
    public static class TypeExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member)
        {
            member.ThrowIfNull("member");

            var attributes = member.GetCustomAttributes(typeof(T), true);
            return attributes.Cast<T>();
        }
    }
}
