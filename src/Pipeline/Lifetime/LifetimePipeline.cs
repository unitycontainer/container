using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{

    [SecuritySafeCritical]
    public class LifetimePipeline : Pipeline
    {
        protected const string error = "\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            return builder.Registration.LifetimeManager switch
            {
                null => TransientLifetime(ref builder),
                TransientLifetimeManager _ => TransientLifetime(ref builder),
                PerResolveLifetimeManager _ => PerResolveLifetime(ref builder),
                ILifetimeManagerAsync _ => AsyncLifetime(ref builder),
                _ => DefaultLifetime(ref builder)
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
                // In Sync mode just execute pipeline
                if (context.Sync) return pipeline(ref context);

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
                object value;
                var lifetime = context.ContainerContext.Lifetime;

                // In Sync mode just execute pipeline
                if (context.Sync)
                {
                    // Get it from context
                    var manager = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    if (null != manager)
                    {
                        value = manager.GetValue(lifetime);
                        if (LifetimeManager.NoValue != value)
                        {
                            return context.Sync ? value : Task.FromResult(value);
                        }
                    }

                    // Compose down the chain
                    value = pipeline(ref context);
                    manager?.SetValue(value, lifetime);

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
                    return pipeline(ref c);
                });
            };
        }

        protected virtual ResolveDelegate<BuilderContext> AsyncLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var manager = builder.Registration.LifetimeManager as ILifetimeManagerAsync;
            var registration = builder.Registration;
            var synchronized = manager as SynchronizedLifetimeManager;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            Debug.Assert(null != manager);

            return (ref BuilderContext context) =>
            {
                var lifetime = context.ContainerContext.Lifetime;

                // In Sync mode just execute pipeline
                if (context.Sync)
                {
                    var value = manager.GetResult(lifetime);
                    if (LifetimeManager.NoValue != value) return value;

                    try
                    {
                        // Compose down the chain
                        value = pipeline(ref context);
                        manager.SetResult(value, lifetime);
                        return value;
                    }
                    catch when (null != synchronized)
                    {
                        synchronized.Recover();
                        throw;
                    }
                }

                // Async mode
                var task = manager.GetTask(lifetime);
                if (null != task) return task;

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
                task = Task.Factory.StartNew(() =>
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

                manager.SetTask(task, lifetime);
                return task;
            };
        }

        protected virtual ResolveDelegate<BuilderContext> DefaultLifetime(ref PipelineBuilder builder)
        {
            var type = builder.Type;
            var name = builder.Name;
            var manager = builder.Registration.LifetimeManager;
            var registration = builder.Registration;
            var synchronized = manager as SynchronizedLifetimeManager;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                var lifetime = context.ContainerContext.Lifetime;

                // In Sync mode just execute pipeline
                if (context.Sync)
                {
                    var value = manager.GetValue(lifetime);
                    if (LifetimeManager.NoValue != value) return value;

                    try
                    {
                        // Compose down the chain
                        value = pipeline(ref context);
                        manager.SetValue(value, lifetime);
                        return value;
                    }
                    catch when (null != synchronized)
                    {
                        synchronized.Recover();
                        throw;
                    }
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
                    var value = manager.GetValue(lifetime);
                    if (LifetimeManager.NoValue != value) return value;

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
                    try
                    {
                        value = pipeline(ref c);
                        manager.SetValue(value, lifetime);
                        return value;
                    }
                    catch when (null != synchronized)
                    {
                        synchronized.Recover();
                        throw;
                    }
                });
            };
        }

        #endregion
    }
}
