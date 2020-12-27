using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Resolve registration throwing exception in case of an error
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(this, ref contract, manager, ref request);

            Policies.ResolveRegistered(ref context);

            if (request.IsFaulted)
            {
                // Throw if user exception
                request.ErrorInfo.Throw();

                // Throw ResolutionFailedException otherwise
                throw new ResolutionFailedException(in contract, request.ErrorInfo.Message);
            }

            return context.Target;
        }

        /// <summary>
        /// Silently resolve registration
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager)
        {
            var request = new RequestInfo(new ResolverOverride[0]);
            var context = new PipelineContext(this, ref contract, manager, ref request);

            Policies.ResolveRegistered(ref context);

            return request.IsFaulted ? null : context.Target;
        }


        private void BuildUpRegistration(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Check if pipeline has been created already
            if (manager.Pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (manager)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (manager.Pipeline is null)
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
                                    manager.Pipeline = Policies.TypePipeline;
                                }

                                break;

                            case RegistrationCategory.Factory:
                                manager.Pipeline = Policies.FactoryPipeline;
                                break;

                            case RegistrationCategory.Instance:
                                manager.Pipeline = Policies.InstancePipeline;
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
                manager.Pipeline!(ref context);
            }
        }
    }
}
