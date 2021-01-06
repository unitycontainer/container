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

            //return  ActivatePipeline;
            var builder = new PipelineBuilder<BuilderContext>(((Policies<BuilderContext>)context.Policies)!.TypeChain);

            return builder.Compile();
            //return builder.Build() ?? UnityContainer.DummyPipeline;
        }
    }
}
