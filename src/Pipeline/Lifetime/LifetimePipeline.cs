using System.Diagnostics;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;
using static Unity.UnityContainer;

namespace Unity
{
    public class LifetimePipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            if (builder.LifetimeManager is ContainerControlledLifetimeManager manager)
            {
                var pipeline = builder.Pipeline();
                Debug.Assert(null != pipeline);

                return (ref BuilderContext context) =>
                {
                    var scope = context.ContainerContext;

                    try
                    {
                        // Switch context to lifetime's scope
                        context.ContainerContext = (ContainerContext)manager.Scope;

                        // Build withing the scope
                        return pipeline(ref context);
                    }
                    finally
                    {
                        context.ContainerContext = scope;
                    }
                };
            }

            return builder.Pipeline(); ;
        }

        #endregion
    }
}
