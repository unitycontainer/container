using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public ref partial struct PipelineBuilder
    {
        private BuildPipelineAsync PerResolveLifetimeAsync()
        {
            var type = Type;
            var name = Name;
            var registration = Registration;
            var pipeline = Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref PipelineContext context) =>
            {
                object value;
                var lifetime = context.ContainerContext.Lifetime;
                var unity = context.ContainerContext;
                var overrides = context.Overrides;

                // In Sync mode just execute pipeline
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
                    };

                    // Compose down the chain
                    value = pipeline(ref c);
                    return new ValueTask<object?>(value);
                }

                // Async mode

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

                // Set Policy storage if required
                if (null == context.List)
                    context.List = new PolicyList();

                // Compose down the chain
                value = pipeline(ref context);
                return new ValueTask<object?>(value);
            };
        }
    }
}
