using System;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
        where TContext : IBuilderContext
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Get and subscribe to Type Pipeline factory
            PipelineFactory = policies.Get<PipelineFactory<TContext>>(typeof(Type), (_, _, policy) 
                => PipelineFactory = (PipelineFactory<TContext>)policy!)!;

            // Register default algorithms for registered and unregistered type resolutions
            policies.Set<ResolveDelegate<TContext>>(typeof(ContainerRegistration), RegisteredAlgorithm);
            policies.Set<ResolveDelegate<TContext>>(UnregisteredAlgorithm);

        }
    }
}
