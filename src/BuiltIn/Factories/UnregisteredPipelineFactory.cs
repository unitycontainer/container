using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class UnregisteredPipelineFactory
    {
        public static ResolveDelegate<ResolutionContext> Factory(ref ResolutionContext context)
        {
            return (ref ResolutionContext context) => null;
        }
    }
}
