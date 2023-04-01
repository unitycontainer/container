using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Algorithms
    {
        /// <summary>
        /// Default algorithm for resolution of registered types
        /// </summary>
        public static object? RegisteredAlgorithm(ref BuilderContext context)
        {
            var manager = context.Registration!;
            var policies = (Policies)context.Policies;

            // Double lock check and create pipeline
            var pipeline = manager.GetPipeline<BuilderContext>(context.Container.Scope);
            if (pipeline is null)
            {
                lock (manager)
                { 
                    if ((pipeline = manager.GetPipeline<BuilderContext>(context.Container.Scope)) is null)
                    {
                        pipeline = !manager.RequireBuild && context.Contract.Type != manager.Type
                            ? policies.MappingPipeline
                            : policies.PipelineFactory(ref context) as ResolveDelegate<BuilderContext>;

                        manager.SetPipeline(pipeline);
                    }
                }
            }

            // Resolve
            pipeline(ref context);

            // Handle errors, if any
            if (context.IsFaulted)
            {
                (manager as SynchronizedLifetimeManager)?.Recover();
                return UnityContainer.NoValue;
            }

            // Save resolved value
            manager.SetValue(context.Existing, context.Container.Scope);

            return context.Existing;
        }
    }
}
