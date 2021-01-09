using System;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
    {
        #region Constants

        const string INVALID_POLICY = "Invalid policy value, the policy can not be null";

        #endregion


        #region Fields

        private static PipelineFactory<TContext>? _typeFactory;
        private static ResolveDelegate<TContext>? _activatePipeline;

        #endregion

        /// <summary>
        /// Default algorithm for unregistered type resolution
        /// </summary>
        public static object? UnregisteredAlgorithm(ref TContext context)
        {
            var type = context.Type;

            // Get pipeline
            var pipeline = context.Policies.CompareExchange(type, _activatePipeline ??= GetActivatePipeline(context.Policies), null);

            if (pipeline is null)
            {
                // Build and save pipeline
                pipeline = (_typeFactory ??= GetTypeFactory(context.Policies))(ref context);

                // TODO: Cache

                context.Policies.CompareExchange(type, pipeline, _activatePipeline);
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


        private static PipelineFactory<TContext> GetTypeFactory(IPolicies policies)
            => policies.Get<PipelineFactory<TContext>>(typeof(Type), (_, _, policy)
                => _typeFactory = (PipelineFactory<TContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy), INVALID_POLICY)))!;


        private static ResolveDelegate<TContext> GetActivatePipeline(IPolicies policies)
            => policies.Get<ResolveDelegate<TContext>>(typeof(Activator), (_, _, policy)
                => _activatePipeline = (ResolveDelegate<TContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy), INVALID_POLICY)))!;
    }
}
