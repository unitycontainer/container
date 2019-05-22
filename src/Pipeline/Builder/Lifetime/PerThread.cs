using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public ref partial struct PipelineBuilder
    {
        private BuildPipelineAsync PerThreadLifetimeAsync()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var manager = Registration?.LifetimeManager;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));


            return (ref PipelineContext context) =>
            {
                // Verify is not async
                if (context.RunAsync || context.RunningAsync)
                {
                    var ex = new InvalidRegistrationException("Lifetime:PerThread is not designed to participate in async operations");
#if NET40 || NET45 || NETSTANDARD1_0
                        var taskSource = new TaskCompletionSource<object?>();
                        taskSource.SetException(ex);
                        var ext = taskSource.Task;
#else
                    var ext = Task.FromException<object?>(ex);
#endif
                    return new ValueTask<object?>(ext);
                }

                Debug.Assert(null != manager);

                // Compose 
                try
                {
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
                    var lifetime = context.ContainerContext.Lifetime;
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
                        Parent = IntPtr.Zero,
                    };

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
                    return new ValueTask<object?>(ext);
                }
            };
        }


        private PipelineDelegate PerThreadLifetime()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var manager = Registration?.LifetimeManager;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                // Verify is not async
                if (context.IsAsync)
                {
                    var ex = new InvalidRegistrationException("Lifetime:PerThread is not designed to participate in async operations");
#if NET40 || NET45 || NETSTANDARD1_0
                    var taskSource = new TaskCompletionSource<object?>();
                    taskSource.SetException(ex);
                    var ext = taskSource.Task;
#else
                    var ext = Task.FromException<object?>(ex);
#endif
                    return new ValueTask<object?>(ext);
                }

                Debug.Assert(null != manager);

                // Compose 
                try
                {
                    var lifetime = context.ContainerContext.Lifetime;
                    var value = manager.GetValue(lifetime);

                    if (LifetimeManager.NoValue != value)
                        return new ValueTask<object?>(value);

                    // Set Policy storage if required
                    if (null == context.List)
                        context.List = new PolicyList();

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
                    return new ValueTask<object?>(ext);
                }
            };
        }
    }
}
