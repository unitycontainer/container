using Unity.Extension;
using Unity.Lifetime;


namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        /// <summary>
        /// Default algorithm for resolution of registered types
        /// </summary>
        internal static object? RegisteredAlgorithm(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            var pipeline = manager.GetPipeline(context.Container.Scope);
            if (pipeline is null) lock (manager) if ((pipeline = manager.GetPipeline(context.Container.Scope)) is null)
            {
                // Create pipeline from context
                pipeline = context.Container.Policies.PipelineFactory(ref context);
                manager.SetPipeline(context.Container.Scope, pipeline);
            }

            // Resolve
            context.Target = pipeline(ref context);

            // Handle errors, if any
            if (context.IsFaulted)
            {
                if (manager is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                return UnityContainer.NoValue;
            }

            // Save resolved value
            manager.SetValue(context.Target, context.Container.Scope);

            return context.Target;
        }


        /// <summary>
        /// Default algorithm for unregistered type resolution
        /// </summary>
        internal static object? UnregisteredAlgorithm(ref PipelineContext context)
        {
            var type = context.Type;
            var defaults = (Policies)context.Policies;

            // Get pipeline
            var pipeline = context.Policies.CompareExchange(type, defaults.ActivatePipeline, null);

            if (pipeline is null)
            {
                // Build and save pipeline with factory
                pipeline = defaults.FromTypeFactory(type);
                context.Policies.CompareExchange(type, pipeline, defaults.ActivatePipeline);
            }

            // Resolve
            context.Target = pipeline!(ref context);

            if (!context.IsFaulted) context.Registration?.SetValue(context.Target, context.Container.Scope);

            return context.Target;
        }
    }
}
