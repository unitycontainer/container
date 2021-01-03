using System;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
        where TContext : IBuilderContext
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            policies.Set<PipelineFactory<TContext>>(PipelineFromRegistrationFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(Type), FromTypeFactory);

            policies.Set<Func<IStagedStrategyChain, ResolveDelegate<TContext>>>(
                                                    PipelineFromStagedChainFactory);
        }
    }
}
