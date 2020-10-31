using System;
using Unity.Container;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constructible

        private void ResolveRegistration(ref PipelineContext context)
        {
            var manager = context.Registration!;
            context.Target = manager.GetValue(_scope);

            if (!ReferenceEquals(RegistrationManager.NoValue, context.Target)) return;
            else context.Target = null; // TODO: context.Target = null

            // Check if pipeline has been created already
            if (null == context.Registration!.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Registration.Pipeline)
                    {
                        using var action = context.Start(context.Registration);

                        context.Registration!.Pipeline = _policies.BuildPipeline(ref context);
                    }
                }
            }

            // Resolve
            using (var action = context.Start(context.Registration!.Data!))
            {
                try
                {
                    context.Target = context.Registration!.Pipeline!(ref context);
                }
                catch (Exception ex)
                {
                    // Unlock the monitor
                    if (context.Registration is SynchronizedLifetimeManager synchronized)
                        synchronized.Recover();

                    // Report telemetry
                    action.Exception(ex);

                    // Re-throw
                    throw; // TODO: replay exception
                }
            }

            if (!context.IsFaulted && manager is LifetimeManager lifetime)
                lifetime.SetValue(context.Target, _scope);
        }

        #endregion


        #region Generic

        private object? GenericRegistration(ref PipelineContext context)
        {
            // Factory manager is in Data
            var manager = context.Registration!;
            var factory = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = factory.Type?.MakeGenericType(context.Contract.Type.GenericTypeArguments);

            ResolveRegistration(ref context);
            
            return context.Target;

            // TODO: 
            //// If any injection members are present, build is required
            //if (manager.RequireBuild) return ResolveRegistration(ref context);

            //// No injectors, redirect
            //return _policies.ResolveMapped(ref context);
        }

        #endregion
    }
}
