using System;
using Unity.Extension;
using Unity.Lifetime;


namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
    {
        #region Fields

        private static PipelineFactory<TContext>? _pipelineFactory;

        #endregion

        /// <summary>
        /// Default algorithm for resolution of registered types
        /// </summary>
        public static object? RegisteredAlgorithm(ref TContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            var pipeline = manager.GetPipeline<TContext>(context.Container.Scope);
            if (pipeline is null) lock (manager) if ((pipeline = manager.GetPipeline<TContext>(context.Container.Scope)) is null)
            {
                // Create pipeline from context
                pipeline = (_pipelineFactory ??= GetPipelineFactory(context.Policies))(ref context);

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


        private static PipelineFactory<TContext> GetPipelineFactory(IPolicies policies) 
            => policies.Get<PipelineFactory<TContext>>((_, _, policy) 
                => _pipelineFactory = (PipelineFactory<TContext>)(policy ?? 
                    throw new ArgumentNullException(nameof(policy), INVALID_POLICY)))!;
    }
}
