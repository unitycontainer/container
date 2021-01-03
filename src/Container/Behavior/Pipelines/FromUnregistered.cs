using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension<TContext>
    {

        public static ResolveDelegate<TContext> FromTypeFactory(ref TContext context)
        {
            switch (context.Registration?.CreationPolicy)
            {
                case CreationPolicy.Any:
                    break;

                case CreationPolicy.Shared:
                    return ((Policies<TContext>)context.Policies).ActivatePipeline;

                case CreationPolicy.NonShared:
                    break;
            }

            //return  ActivatePipeline;
            var builder = new PipelineBuilder<TContext>(((Policies<TContext>)context.Policies)!.TypeChain);

            return builder.Compile();
            //return builder.Build() ?? UnityContainer.DummyPipeline;
        }
    }
}
