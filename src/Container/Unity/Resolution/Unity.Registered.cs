using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constructible

        private object? ResolveRegistration(ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            var context = new PipelineContext(this, ref contract, manager, ref parent);

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
                    context.Registration!.Pipeline!(ref context);
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

            // Handle result
            // TODO: if (context.IsFaulted) throw new ResolutionFailedException(context.Type, context.Name, context.Error!);
            context.LifetimeManager?.SetValue(context.Target, _scope.Disposables);

            // Return resolved
            return context.Target;
        }

        private object? ResolveRegistration(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = new PipelineRequest(overrides);
            var context = new PipelineContext(this, ref contract, manager, ref request);

            // Check if pipeline has been created already
            if (null == context.Registration!.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Registration.Pipeline)
                    {
                        using var action = context.Start(manager);

                        context.Registration!.Pipeline = _policies.BuildPipeline(ref context);
                    }
                }
            }

            // Resolve
            using (var action = context.Start(manager.Data!))
            {
                try
                {
                    context.Registration!.Pipeline!(ref context);
                }
                catch (Exception ex)
                {
                    // Unlock the monitor
                    if (manager is SynchronizedLifetimeManager synchronized)
                        synchronized.Recover();

                    // Report telemetry
                    action.Exception(ex);

                    // Re throw
                    throw; // TODO: replay exception
                }
            }

            // Handle result
            //if (request.IsFaulted) throw new ResolutionFailedException(contract.Type, contract.Name, request.Error!);
            if (manager is LifetimeManager lifetime) lifetime.SetValue(context.Target, _scope.Disposables);

            // Return resolved
            return context.Target;
        }

        #endregion


        #region Generic

        private object? GenericRegistration(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var info = new PipelineRequest(overrides);
            var context = new PipelineContext(this, ref contract, manager, ref info);
            var factory = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = factory.Type?.MakeGenericType(contract.Type.GenericTypeArguments);

            // If any injection members are present, build is required
            if (manager.RequireBuild) return _policies.ResolveContract(ref context);

            // No injectors, redirect
            return _policies.ResolveMapped(ref context);
        }


        private object? GenericRegistration(ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            throw new NotImplementedException();

            //var info = new RequestInfo(overrides);
            //var context = new PipelineContext(this, ref contract, manager, ref info);
            //var factory = (RegistrationManager)manager.Data!;

            //// Calculate new Type
            //manager.Category = RegistrationCategory.Type;
            //manager.Data = factory.Type?.MakeGenericType(contract.Type.GenericTypeArguments);

            //// If any injection members are present, build is required
            //if (manager.RequireBuild) return _policies.ResolveContract(ref context);

            //// No injectors, redirect
            //return _policies.ResolveMapped(ref context);
        }

        #endregion
    }
}
