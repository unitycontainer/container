using System;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class OptimizedPipelineFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;

            policies.Set(typeof(Defaults.OptimizedPipelineFactory), (Defaults.OptimizedPipelineFactory)Factory);
        }

        public static Pipeline Factory(ref ResolutionContext context)
        {
            return (ref ResolutionContext c) => throw new Exception();
        }
    }
}
