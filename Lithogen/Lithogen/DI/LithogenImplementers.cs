using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lithogen.DI
{
    [DebuggerDisplay("{InterfaceType.FullName}")]
    class LithogenImplementers
    {
        public Type InterfaceType { get; set; }
        public IEnumerable<Type> Implementers { get; set; }
    }
}
