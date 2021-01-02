using Unity.Container;
using Unity.Lifetime;


namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        /// <summary>
        /// Default algorithm for resolution of registered types
        /// </summary>
        public static object? RegisteredAlgorithm(ref PipelineContext context)
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
    }
}
