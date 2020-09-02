using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class BalancedPipelineFactory
    {
        public static ResolveDelegate<ResolutionContext> Factory(ref ResolutionContext context)
        {
            return (ref ResolutionContext c) => null;
        }
    }
}
