using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Resolve existing registration
        /// </summary>
        private object? ResolveRegistered(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            if (manager.Pipeline is null) lock (manager) if (manager.Pipeline is null)
            { 
                // Create pipeline from context
                manager.Pipeline = Policies.ContextualFactory(ref context);
            }

            // Resolve
            try
            {
                using var scope = context.CreateScope(this);

                context.Target = manager.Pipeline!(ref context);
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }

            // Handle errors, if any
            if (context.IsFaulted)
            {
                if (manager is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                return RegistrationManager.NoValue;
            }

            // Save resolved value
            manager.SetValue(context.Target, Scope);

            return context.Target;
        }


        /// <summary>
        /// Resolve unregistered
        /// </summary>
        private object? ResolveUnregistered(ref PipelineContext context)
        {
            var type = context.Type;
            
            // Get pipeline
            if (!Policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                // Build and save pipeline with factory
                pipeline = Policies.PipelineFactory(type);
                pipeline = Policies.GetOrAdd(type, pipeline);
            }

            // Resolve
            using (var scope = context.CreateScope(this))
            { 
                context.Target = pipeline!(ref context);
            }

            if (!context.IsFaulted) context.Registration?.SetValue(context.Target, Scope);

            return context.Target;
        }
    }
}
