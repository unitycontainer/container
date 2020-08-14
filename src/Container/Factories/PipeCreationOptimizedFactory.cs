using Unity.Pipeline;
using Unity.Resolution;

namespace Unity.Container
{
    public static class PipeCreationOptimizedFactory
    {
        public static ResolveDelegate<ResolveContext> Factory(ref ResolveContext context)
        {
            return (ref ResolveContext context) => null;
        }
    }
}
