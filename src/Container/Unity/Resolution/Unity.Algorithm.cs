using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private void ResolveRegistration(ref PipelineContext context)
        {
            var manager = context.Registration!;
            context.Target = manager.GetValue(_scope);

            if (!ReferenceEquals(RegistrationManager.NoValue, context.Target)) return;
            else context.Target = null; // TODO: context.Target = null

            // Check if pipeline has been created already
            if (null == manager.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (manager)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (null == manager.Pipeline)
                    {
                        using var action = context.Start(manager);

                        switch (manager.Category)
                        {
                            case RegistrationCategory.Type:

                                // Check for Type Mapping
                                var registration = context.Registration;
                                if (null != registration && !registration.RequireBuild && context.Contract.Type != registration.Type)
                                {
                                    var contract = new Contract(registration.Type!, context.Contract.Name);

                                    manager.Pipeline = (ref PipelineContext c) =>
                                    {
                                        var stack = contract;
                                        var local = c.CreateContext(ref stack);

                                        c.Target = local.Resolve();

                                        return c.Target;
                                    };
                                }
                                else
                                { 
                                    manager.Pipeline = _policies.BuildTypePipeline(context.Contract.Type);
                                }

                                break;

                            case RegistrationCategory.Factory:
                                manager.Pipeline = _policies.BuildFactoryPipeline(context.Contract.Type);
                                break;

                            case RegistrationCategory.Instance:
                                manager.Pipeline = _policies.BuildInstancePipeline(context.Contract.Type);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }

            // Resolve
            using (var action = context.Start(manager.Data!))
            {
                try
                {
                    context.Target = manager.Pipeline!(ref context);
                }
                catch (Exception ex)
                {
                    // Unlock the monitor
                    if (manager is SynchronizedLifetimeManager synchronized)
                        synchronized.Recover();

                    // Report telemetry
                    action.Exception(ex);

                    // TODO: Re-throw ??
                    throw; // TODO: replay exception
                }
            }

            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);
        }


        private object? ResolveUnregistered(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
                pipeline = _policies.AddOrGet(type, _policies.BuildPipeline(context.Contract.Type));

            // Resolve
            try
            {
                context.Target = pipeline!(ref context);
            }
            catch (Exception ex)
            {
                // Unlock the monitor
                if (context.Registration is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                // TODO: Report telemetry
                context.Exception(ex);

                // TODO: Re-throw ??
                throw; // TODO: replay exception
            }
            
            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);

            return context.Target;
        }
    }
}
