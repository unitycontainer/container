using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class OptimizedPipelineFactory
    {
        public static ResolveDelegate<ResolutionContext> Factory(ref ResolutionContext context)
        {
            return (ref ResolutionContext c) => null;
        }
    }
}
