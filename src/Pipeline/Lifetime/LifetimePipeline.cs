using System.Diagnostics;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public class LifetimePipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            ResolveDelegate<BuilderContext>? pipeline = builder.Pipeline();
            Debug.Assert(null != pipeline);

            return builder.LifetimeManager is SynchronizedLifetimeManager manager ?
            (ref BuilderContext context) =>
            {
                try
                {
                    // Build withing the scope
                    return pipeline(ref context);
                }
                catch
                {
                    manager.Recover();
                    throw;
                }
            }
            : pipeline;
        }

        #endregion
    }
}
