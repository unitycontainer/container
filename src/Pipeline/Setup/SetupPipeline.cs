using System;
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
        protected const string error = "\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            return builder.Registration.LifetimeManager switch
            {
                null                        => TransientLifetime(ref builder),
                TransientLifetimeManager  _ => TransientLifetime(ref builder),
                PerResolveLifetimeManager _ => PerResolveLifetime(ref builder),
                _                           => DefaultLifetime(ref builder)
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
                catch (Exception ex) when (null == context.DeclaringType && ( ex is InvalidRegistrationException || ex is CircularDependencyException))
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
            var registration = builder.Registration;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    object value;

                    // Get it from context
                    var manager = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    if (null != manager)
                    {
                        value = manager.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value)
                        {
                            return context.Async 
                                ? Task.FromResult(value)
                                : value;
                        }
                    }

                    // In Sync mode just execute pipeline
                    if (!context.Async)
                    {
                        // Compose down the chain
                        value = pipeline(ref context);
                        manager?.SetValue(value, context.ContainerContext.Lifetime);

                        return value;
                    }

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
                        value = pipeline(ref c);
                        manager?.SetValue(value, c.ContainerContext.Lifetime);

                        return value;
                    });
                }
                catch (Exception ex) when (null == context.DeclaringType && (ex is InvalidRegistrationException || ex is CircularDependencyException))
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}" + error, ex);
                }
            };
        }

        protected virtual ResolveDelegate<BuilderContext> DefaultLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var lifetime = builder.Registration.LifetimeManager;
            var registration = builder.Registration;
            var synchronized = lifetime as SynchronizedLifetimeManager;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    // Return if holds value
                    object value = lifetime.GetValue(context.ContainerContext.Lifetime);

                    if (LifetimeManager.NoValue != value)
                        return context.Async ? Task.FromResult(value) : value;

                    // In Sync mode just execute pipeline
                    if (!context.Async)
                    {
                        value = pipeline(ref context);

                        // Set value
                        lifetime.SetValue(value, context.ContainerContext.Lifetime);
                        return value;
                    }

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
                    var task = Task.Factory.StartNew(() =>
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
                        var result = pipeline(ref c);

                        lifetime.SetValue(result, c.ContainerContext.Lifetime);

                        return result;
                    });

                    return task;
                }
                catch (Exception ex)
                {
                    synchronized?.Recover();

                    if (null == context.DeclaringType && (ex is InvalidRegistrationException || ex is CircularDependencyException))
                    {
                        throw new ResolutionFailedException(context.Type, context.Name,
                            $"Resolution failed with error: {ex.Message}" + error, ex);
                    }
                    else throw;
                }
            };
        }

        #endregion
    }
}
