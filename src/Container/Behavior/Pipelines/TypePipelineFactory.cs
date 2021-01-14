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

            policies.Set<PipelineFactory<BuilderContext>>(typeof(Type), DefaultFactory);
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


        public static ResolveDelegate<BuilderContext> PipelineResolved(ref BuilderContext context)
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

            var chain = ((Policies<BuilderContext>)context.Policies)!.TypeChain;
            var builder = new PipelineBuilder<BuilderContext>(chain);

            return builder.Build(ref context) ?? UnityContainer.DummyPipeline;
        }


        public static ResolveDelegate<BuilderContext> PipelineCompiled(ref BuilderContext context)
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

            var chain = ((Policies<BuilderContext>)context.Policies)!.TypeChain;
            var builder = new PipelineBuilder<BuilderContext>(chain);

            return builder.Compile();
        }
    }
}
