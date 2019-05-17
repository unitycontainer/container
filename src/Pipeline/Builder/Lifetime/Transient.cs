using System;
using System.Threading.Tasks;
using Unity.Builder;

namespace Unity
{
    public ref partial struct PipelineBuilder
    {
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
    }
}
