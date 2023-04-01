using Unity.Builder;
using Unity.Lifetime;

namespace Unity.Container
{
    internal static partial class Algorithms
    {
        /// <summary>
        /// Default algorithm for unregistered type resolution
        /// </summary>
        public static object? UnregisteredAlgorithm(ref BuilderContext context)
        {
            var type = context.Type;
            var policies = (Policies)context.Policies;
            
            // TODO: This only works when compilation is available
            
            // Get pipeline, if not yet set, put a built-in until real pipeline if built
            var pipeline = context.Policies.CompareExchange(type, policies.ActivatePipeline, null);

            if (pipeline is null)
            {
                // Build and save pipeline
                pipeline = policies.PipelineFactory(ref context);

                // Save pipeline, discard if already set by other thread
                context.Policies.CompareExchange(type, pipeline, policies.ActivatePipeline);
            }

            // Resolve
            pipeline!(ref context);

            if (context.IsFaulted)
            {
                (context.Registration as SynchronizedLifetimeManager)?.Recover();
                return UnityContainer.NoValue;
            }
                
            context.Registration?.SetValue(context.Existing, context.Container.Scope);

            return context.Existing;
        }
    }
}
