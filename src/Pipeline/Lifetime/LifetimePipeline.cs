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

            return builder.LifetimeManager switch
            {
                SynchronizedLifetimeManager manager => SynchronizedLifetime(manager, pipeline),
                PerResolveLifetimeManager _ => PerResolveLifetime(pipeline),
                _ => pipeline
            };
        }

        #endregion

        private ResolveDelegate<BuilderContext> SynchronizedLifetime(SynchronizedLifetimeManager manager, ResolveDelegate<BuilderContext> pipeline)
        {
            return (ref BuilderContext context) =>
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

        private ResolveDelegate<BuilderContext> PerResolveLifetime(ResolveDelegate<BuilderContext> pipeline)
        {
            return (ref BuilderContext context) =>
            {
                object?          value;
                LifetimeManager? lifetime;

                if (null != (lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager))))
                {
                    value = lifetime.Get(context.ContainerContext.Lifetime);
                    if (LifetimeManager.NoValue != value) return value;
                }

                value = pipeline(ref context);

                if (null == (lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager))))
                {
                    context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));
                }

                return value;
            };
        }
    }
}
