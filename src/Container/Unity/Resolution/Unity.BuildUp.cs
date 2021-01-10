using System;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        private void BuildUpRegistration(ref BuilderContext context)
        {
            var manager = context.Registration!;

            // Check if pipeline has been created already
            var pipeline = manager.GetPipeline<BuilderContext>(context.Container.Scope);
            if (pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (manager)
                {
                    // Make sure it is still null and not created while waited for the lock
                    pipeline = manager.GetPipeline<BuilderContext>(context.Container.Scope);
                    if (pipeline is null)
                    {
                        using var action = context.Start(manager);

                        switch (manager.Category)
                        {
                            case RegistrationCategory.Type:

                                var registration = context.Registration;
                                
                                // Check for Type Mapping
                                if (null != registration && !registration.RequireBuild && context.Contract.Type != registration.Type)
                                {
                                    var contract = new Contract(registration.Type!, context.Contract.Name);

                                    // Mapping resolver
                                    pipeline = manager.SetPipeline(context.Container.Scope, (ref BuilderContext c) => c.MapTo(contract))!;
                                }
                                else
                                {
                                    pipeline = manager.SetPipeline(context.Container.Scope, Policies.ActivatePipeline)!;
                                }

                                break;

                            case RegistrationCategory.Factory:
                                pipeline = manager.SetPipeline(context.Container.Scope, Policies.FactoryPipeline)!;
                                break;

                            case RegistrationCategory.Instance:
                                pipeline = manager.SetPipeline(context.Container.Scope, Policies.InstancePipeline)!;
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
                pipeline(ref context);
            }
        }
    }
}
