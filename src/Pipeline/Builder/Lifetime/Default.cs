using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public ref partial struct PipelineBuilder
    {
        private BuildPipelineAsync DefaultLifetimeAsync()
        {
            var type = Type;
            var name = Name;
            var manager = Registration?.LifetimeManager;
            var registration = Registration;
            var synchronized = manager as SynchronizedLifetimeManager;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));
            Debug.Assert(null != manager);

            return (ref PipelineContext context) =>
            {
                var lifetime = context.ContainerContext.Lifetime;
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

                // In Sync mode just execute pipeline
                if (!context.RunAsync)
                {
                    var value = manager.GetValue(lifetime);
                    if (LifetimeManager.NoValue != value)
                        return new ValueTask<object?>(value);

                    var c = new BuilderContext
                    {
                        List = new PolicyList(),
                        IsAsync = true,
                        Type = type,
                        ContainerContext = unity,
                        Registration = registration,
                        Overrides = overrides,
                        DeclaringType = type,
                    };

                    try
                    {
                        // Compose down the chain
                        value = pipeline(ref c);
                        manager.SetValue(value, lifetime);
                        return new ValueTask<object?>(value);
                    }
                    catch (Exception ex)// when (null != synchronized)
                    {
#if NET40 || NET45 || NETSTANDARD1_0
                        var taskSource = new TaskCompletionSource<object?>();
                        taskSource.SetException(ex);
                        var ext = taskSource.Task;
#else
                        var ext = Task.FromException<object?>(ex);
#endif
                        synchronized?.Recover();
                        return new ValueTask<object?>(ext);
                    }
                }

                // Async mode

                // Create and return a task that creates an object
                var task = Task.Factory.StartNew<object?>(() =>
                {
                    var value = manager.GetValue(lifetime);
                    if (LifetimeManager.NoValue != value) return value;

                    var c = new BuilderContext
                    {
                        List = new PolicyList(),
                        IsAsync = true,
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


        private PipelineDelegate DefaultLifetime()
        {
            var type = Type;
            var name = Name;
            var manager = Registration?.LifetimeManager;
            var registration = Registration;
            var synchronized = manager as SynchronizedLifetimeManager;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));
            Debug.Assert(null != manager);

            return (ref BuilderContext context) =>
            {
                var lifetime = context.ContainerContext.Lifetime;

                var value = manager.GetValue(lifetime);
                if (LifetimeManager.NoValue != value)
                    return new ValueTask<object?>(value);

                // Set Policy storage if required
                if (null == context.List)
                    context.List = new PolicyList();

                try
                {
                    // Compose down the chain
                    value = pipeline(ref context);
                    manager.SetValue(value, lifetime);
                    return new ValueTask<object?>(value);
                }
                catch (Exception ex)// when (null != synchronized)
                {
#if NET40 || NET45 || NETSTANDARD1_0
                    var taskSource = new TaskCompletionSource<object?>();
                    taskSource.SetException(ex);
                    var ext = taskSource.Task;
#else
                    var ext = Task.FromException<object?>(ex);
#endif
                    synchronized?.Recover();
                    return new ValueTask<object?>(ext);
                }
            };
        }
    }
}
