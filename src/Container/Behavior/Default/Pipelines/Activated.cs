using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineActivated(ref BuilderContext context)
        {
            return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;
        }
    }
}
