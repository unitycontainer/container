using Unity.Extension;
using Unity.Lifetime;


namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        /// <summary>
        /// Default algorithm for unregistered type resolution
        /// </summary>
        internal static object? UnregisteredPipeline(ref PipelineContext context)
        {
            var type = context.Type;
            var defaults = (Policies)context.Policies;

            // Get pipeline
            var pipeline = context.Policies.CompareExchange(type, defaults.TypePipeline, null);

            if (pipeline is null)
            {
                // Build and save pipeline with factory
                pipeline = defaults.FromTypeFactory(type);
                pipeline = context.Policies.CompareExchange(type, pipeline, defaults.TypePipeline);
            }

            // Resolve
            context.Target = pipeline!(ref context);

            if (!context.IsFaulted) context.Registration?.SetValue(context.Target, context.Container.Scope);

            return context.Target;
        }


        /// <summary>
        /// Actual resolution method
        /// </summary>
        internal static object? RegisteredPipeline(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            if (manager.Pipeline is null) lock (manager) if (manager.Pipeline is null)
            {
                // Create pipeline from context
                manager.Pipeline = context.Container.Policies.PipelineFactory(ref context);
            }

            // Resolve
            context.Target = manager.Pipeline!(ref context);

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

    }
}
