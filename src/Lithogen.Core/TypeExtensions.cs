using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithogen.Core
{
    public static class TypeExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(T), true);
            return attributes.Cast<T>();
        }
    }
}
