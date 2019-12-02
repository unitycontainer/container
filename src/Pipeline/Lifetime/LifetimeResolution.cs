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
                SynchronizedLifetimeManager manager => SynchronizedLifetimeResolution(manager, pipeline!),
                PerResolveLifetimeManager _ => PerResolveLifetimeResolution(pipeline!),
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
                    // Execute Pipeline
                    return pipeline(ref context);
                }
                catch
                {
                    // Recover and rethrow
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

                // Check and return if already resolved
                LifetimeManager? lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));
                if (null != lifetime)
                {
                    value = lifetime.Get(context.LifetimeContainer);
                    if (LifetimeManager.NoValue != value) return value;
                }

                // Execute Pipeline
                value = pipeline(ref context);

                // Save resolved value in per resolve singleton
                if (null != context.DeclaringType)
                    context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));

                return value;
            };
        }

        #endregion
    }
}
