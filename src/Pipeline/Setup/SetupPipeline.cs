using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class SetupPipeline : Pipeline 
    {
        protected const string error = "\n\nFor more detailed information run Unity in debug mode: new UnityContainer(enableDiagnostic: true)";

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            return builder.Registration.LifetimeManager switch
            {
                null                          => TransientLifetime(ref builder),
                TransientLifetimeManager    _ => TransientLifetime(ref builder),
                //PerResolveLifetimeManager   _ => PerResolveLifetime(ref builder),
                //SynchronizedLifetimeManager _ => SynchronizedLifetime(ref builder),
                _                             => DefaultLifetime(ref builder)
            };
        }


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext> TransientLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var registration = builder.Registration;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    // In Sync mode just execute pipeline
                    if (!context.Async) return pipeline(ref context);

                    // Async mode
                    var list = context.List;
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
                    IntPtr parent = IntPtr.Zero;
#if !NET40
                    unsafe
                    {
                        var thisContext = this;
                        parent = new IntPtr(Unsafe.AsPointer(ref thisContext));
                    }
#endif
                    // Create and return a task that creates an object
                    return Task.Factory.StartNew(() =>
                    {
                        var c = new BuilderContext
                        {
                            List = list,
                            Type = type,
                            ContainerContext = unity,
                            Registration = registration,
                            Overrides = overrides,
                            DeclaringType = type,
                            Parent = parent,
                        };

                        // Execute pipeline
                        return pipeline(ref c);
                    });
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException &&
                                           null == context.DeclaringType)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}" + error, ex);
                }
            };
        }

        protected virtual ResolveDelegate<BuilderContext> PerResolveLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    object value;

                    // Get it from context
                    var policy = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    if (null != policy)
                    {
                        value = policy.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value) return value;
                    }

                    // Compose down the chain
                    value = pipeline(ref context);
                    policy?.SetValue(value, context.ContainerContext.Lifetime);

                    return value;
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException &&
                                           null == context.DeclaringType)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}" + error, ex);
                }
            };
        }

        protected virtual ResolveDelegate<BuilderContext> SynchronizedLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var lifetime = builder.Registration.LifetimeManager as SynchronizedLifetimeManager;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            Debug.Assert(null != lifetime);

            return (ref BuilderContext context) =>
            {
                try
                {
                    try
                    {
                        // Return if holds value
                        var value = lifetime.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value) return value;

                        // Compose 
                        value = pipeline(ref context);

                        // Set value
                        lifetime.SetValue(value, context.ContainerContext.Lifetime);
                        return value;
                    }
                    catch when (null != lifetime)
                    {
                        lifetime.Recover();
                        throw;
                    }
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException &&
                                           null == context.DeclaringType)
                {
                    lifetime.Recover();
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}" + error, ex);
                }
            };
        }

        protected virtual ResolveDelegate<BuilderContext> DefaultLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    // Build the type
                    return pipeline(ref context);
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException &&
                                           null == context.DeclaringType)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}", ex);
                }
            };
        }

        #endregion
    }
}
