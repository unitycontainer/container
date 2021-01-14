using System;
using System.ComponentModel.Composition;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            policies.Set<PipelineFactory<BuilderContext>>(DefaultFactory);
        }



        public static ResolveDelegate<BuilderContext> DefaultFactory(ref BuilderContext context)
        {
            switch (context.Registration?.CreationPolicy)
            {
                case CreationPolicy.Any:
                    break;

                case CreationPolicy.Shared:
                    return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;

                case CreationPolicy.NonShared:
                    break;
            }

            // TODO: Implement smart selection
            return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;
        }
    }
}
