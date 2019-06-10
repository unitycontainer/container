using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public class LifetimePipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            ResolveDelegate<PipelineContext>? pipeline = builder.Pipeline();
            Debug.Assert(null != pipeline);

            return builder.LifetimeManager switch
            {
                SynchronizedLifetimeManager manager => SynchronizedLifetime(manager, pipeline),
                PerResolveLifetimeManager _ => PerResolveLifetime(pipeline),
                _ => pipeline
            };
        }

        #endregion

        private ResolveDelegate<PipelineContext> SynchronizedLifetime(SynchronizedLifetimeManager manager, ResolveDelegate<PipelineContext> pipeline)
        {
            return (ref PipelineContext context) =>
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
            };
        }

        private ResolveDelegate<PipelineContext> PerResolveLifetime(ResolveDelegate<PipelineContext> pipeline)
        {
            return (ref PipelineContext context) =>
            {
                object?          value;
                LifetimeManager? lifetime;

                if (null != (lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager))))
                {
                    value = lifetime.Get(context.ContainerContext.Lifetime);
                    if (LifetimeManager.NoValue != value) return value;
                }

                value = pipeline(ref context);

                if (null != context.DeclaringType)
                    context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));

                return value;
            };
        }
    }
}
