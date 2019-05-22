using System;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Storage;

namespace Unity
{
    public ref partial struct PipelineBuilder
    {
        private BuildPipelineAsync TransientLifetimeAsync()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref PipelineContext context) =>
            {
                // Async mode
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

                //In Sync mode just execute pipeline
                if (!context.RunAsync)
                {
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

                    // Execute pipeline
                    var value = pipeline(ref c);
                    return new ValueTask<object?>(value);
                }

                // Create and return a task that creates an object
                var task = Task.Factory.StartNew<object?>(() =>
                {
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

                    // Execute pipeline
                    return pipeline(ref c);
                });

                return new ValueTask<object?>(task);
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
                // Set Policy storage if required
                if (null == context.List)
                    context.List = new PolicyList();

                var value = pipeline(ref context);
                return new ValueTask<object?>(value);
            };
        }
    }
}
