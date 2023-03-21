using Unity.Extension;
using Unity.Resolution;

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
                case CreationPolicy.Once:
                    return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;

                case CreationPolicy.Always:
                    return PipelineCompiled(ref context);

                case CreationPolicy.OnceInWhile:
                    return PipelineResolved(ref context);
            }

            return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;
        }
    }
}
