using System.ComponentModel.Composition;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> FromTypeFactory(ref BuilderContext context)
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

        public static ResolveDelegate<BuilderContext> FromTypeActivated(ref BuilderContext context)
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


        public static ResolveDelegate<BuilderContext> FromTypeResolved(ref BuilderContext context)
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

            var builder = new PipelineBuilder<BuilderContext>(((Policies<BuilderContext>)context.Policies)!.TypeChain);

            return builder.Build() ?? UnityContainer.DummyPipeline;
        }


        public static ResolveDelegate<BuilderContext> FromTypeCompiled(ref BuilderContext context)
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

            var builder = new PipelineBuilder<BuilderContext>(((Policies<BuilderContext>)context.Policies)!.TypeChain);

            return builder.Compile();
        }
    }
}
