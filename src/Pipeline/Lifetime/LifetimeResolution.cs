using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class LifetimePipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            ResolveDelegate<PipelineContext>? pipeline = builder.Pipeline();
            Debug.Assert(null != pipeline);

            return builder.LifetimeManager switch
            {
                SynchronizedLifetimeManager manager => SynchronizedLifetimeResolution(manager, pipeline),
                PerResolveLifetimeManager _ => PerResolveLifetimeResolution(pipeline),
                _ => pipeline
            };
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext> SynchronizedLifetimeResolution(SynchronizedLifetimeManager manager, ResolveDelegate<PipelineContext> pipeline)
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

        protected virtual ResolveDelegate<PipelineContext> PerResolveLifetimeResolution(ResolveDelegate<PipelineContext> pipeline)
        {
            return (ref PipelineContext context) =>
            {
                object? value;
                LifetimeManager? lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                if (null != lifetime)
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

        #endregion
    }
}
