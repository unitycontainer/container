using System;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            policies.Set<PipelineFactory<BuilderContext>>(PipelineFromRegistrationFactory);
            policies.Set<PipelineFactory<BuilderContext>>(typeof(Type), FromTypeFactory);
            
            policies.Set<Func<IStagedStrategyChain, ResolveDelegate<BuilderContext>>>(
                                                    PipelineFromStagedChainFactory);
        }
    }
}
