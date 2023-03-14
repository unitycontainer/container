using System.ComponentModel.Composition;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineActivated(ref BuilderContext context)
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

            return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;
        }
    }
}
