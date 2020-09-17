using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class UnregisteredPipelineFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;

            policies.Set(typeof(Defaults.UnregisteredPipelineFactory), (Defaults.UnregisteredPipelineFactory)Factory);
        }

        public static Pipeline Factory(ref ResolutionContext context)
        {
            return (ref ResolutionContext c) => null;
        }
    }
}
