using System;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;

namespace Unity
{
    [SecuritySafeCritical]
    public ref partial struct PipelineBuilder
    {
        private const string error = "\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";

        public PipelineDelegate PipelineDelegate()
        {
            var builder = this;
            return Registration.LifetimeManager switch
            {
                null                        => TransientLifetime(),
                TransientLifetimeManager  _ => TransientLifetime(),
                PerResolveLifetimeManager _ => PerResolveLifetime(),
                //ILifetimeManagerAsync _ => AsyncLifetime(ref builder),
                _ => DefaultLifetime()
            };
        }

        private PipelineDelegate TransientLifetime()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                //In Sync mode just execute pipeline
                if (context.Sync)
                {
                    var value = pipeline(ref context);
                    return new ValueTask<object?>(value);
                }

                // Async mode
                var list = context.List;
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

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
                        Parent = IntPtr.Zero,
                    };

                    // Execute pipeline
                    return pipeline(ref c);
                });

                return new ValueTask<object?>(task);
            };
        }

        private PipelineDelegate PerResolveLifetime()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

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
                            return new ValueTask<object?>(value);
                        }
                    }

                    // Compose down the chain
                    value = pipeline(ref context);
                    return new ValueTask<object?>(value);
                }

                // Async mode
                var list = context.List;
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

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
                    };

                    // Execute pipeline
                    return pipeline(ref c);
                });

                return new ValueTask<object?>(task);
            };
        }

        private PipelineDelegate DefaultLifetime()
        {
            var type = Type;
            var name = Name;
            var manager = Registration.LifetimeManager;
            var registration = Registration;
            var synchronized = manager as SynchronizedLifetimeManager;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                var lifetime = context.ContainerContext.Lifetime;

                // In Sync mode just execute pipeline
                if (context.Sync)
                {
                    var value = manager.GetValue(lifetime);
                    if (LifetimeManager.NoValue != value)
                        return new ValueTask<object?>(value);

                    try
                    {
                        // Compose down the chain
                        value = pipeline(ref context);
                        manager.SetValue(value, lifetime);
                        return new ValueTask<object?>(value);
                    }
                    catch (Exception ex)// when (null != synchronized)
                    {
                        synchronized?.Recover();
                        //throw;

#if NET40 || NET45 || NETSTANDARD1_0
                        var taskSource = new TaskCompletionSource<object?>();
                        taskSource.SetException(ex);
                        var ext = taskSource.Task;
#else
                        var ext = Task.FromException<object?>(ex);
#endif
                        return new ValueTask<object?>(ext);
                    }
                }

                // Async mode
                var list = context.List;
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

                // Create and return a task that creates an object
                var task = Task.Factory.StartNew(() =>
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
                return new ValueTask<object?>(task);
            };
        }
    }
}
