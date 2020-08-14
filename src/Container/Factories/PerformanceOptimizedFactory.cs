using System;
using System.Collections.Generic;
using System.Text;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity.Container
{
    public static class PerformanceOptimizedFactory
    {
        public static ResolveDelegate<ResolveContext> Factory(ref ResolveContext context)
        {
            return (ref ResolveContext context) => null;
        }
    }
}
