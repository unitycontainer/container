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
                SynchronizedLifetimeManager manager => SynchronizedLifetimeResolution(pipeline, manager),
                PerResolveLifetimeManager _         => PerResolveLifetimeResolution(pipeline!, builder.LifetimeManager?.Scope),
                _                                   => OtherLifetimeResolution(pipeline, builder.LifetimeManager?.Scope)
            };
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext> SynchronizedLifetimeResolution(ResolveDelegate<PipelineContext> pipeline, SynchronizedLifetimeManager manager)
        {
            if (null == manager.Scope)
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
            else
            {
                return (ref PipelineContext context) =>
                {
                    var containerContext = context.ContainerContext;
                    try
                    {
                        // Execute Pipeline with new scope
                        context.ContainerContext = (UnityContainer.ContainerContext)manager.Scope;
                        return pipeline(ref context);
                    }
                    catch
                    {
                        // Recover and rethrow
                        manager.Recover();
                        throw;
                    }
                    finally
                    {
                        // Recover context
                        context.ContainerContext = containerContext;
                    }
                };
            }
        }

        protected virtual ResolveDelegate<PipelineContext> PerResolveLifetimeResolution(ResolveDelegate<PipelineContext> pipeline, object? scope)
        {
            if (null == scope)
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
            else
            {
                return (ref PipelineContext context) =>
                {
                    object? value;
                    var containerContext = context.ContainerContext;
                    
                    try
                    {
                        // Execute Pipeline with new scope
                        context.ContainerContext = (UnityContainer.ContainerContext)scope;

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
                    }
                    finally
                    {
                        // Recover context
                        context.ContainerContext = containerContext;
                    }

                    return value;
                };
            }
        }

        protected virtual ResolveDelegate<PipelineContext> OtherLifetimeResolution(ResolveDelegate<PipelineContext> pipeline, object? scope)
        {
            if (null == scope) return pipeline;

            return (ref PipelineContext context) =>
            {
                var containerContext = context.ContainerContext;
                try
                {
                    // Execute Pipeline with new scope
                    context.ContainerContext = (UnityContainer.ContainerContext)scope;
                    return pipeline(ref context);
                }
                finally
                {
                    // Recover context
                    context.ContainerContext = containerContext;
                }
            };
        }

        #endregion
    }
}
