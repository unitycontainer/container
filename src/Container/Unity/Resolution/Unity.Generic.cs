using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private object? GenericRegistration(ref PipelineContext context)
        {
            // Factory manager is in Data
            var manager = context.Registration!;
            var factory = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = factory.Type?.MakeGenericType(context.Contract.Type.GenericTypeArguments);

            // Create mapping if nothing to build
            if (!manager.RequireBuild && context.Contract.Type != manager.Type)
            {
                var closure = new Contract(manager.Type!, context.Contract.Name);
                // Mapping resolver
                manager.Pipeline = (ref PipelineContext c) =>
                {
                    var contract = closure;
                    return c.Container.Resolve(ref contract, ref c);
                };
            }

            ResolveRegistration(ref context);

            return context.Target;
        }

        /// <summary>
        /// Initialize newly create Bound Generic registration
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> to build</param>
        /// <param name="definition">Generic <see cref="Type"/> definition</param>
        /// <param name="manager">New <see cref="RegistrationManager"/> created in previous step</param>
        /// <param name="parent"><see cref="PipelineContext"/> of last step</param>
        /// <returns>Resolved value</returns>
        private object? FromUnboundGenericRegistration(ref Contract contract, Type definition, RegistrationManager manager, ref PipelineContext parent)
        {
            // Unbound generic manager
            var unbound = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = unbound.Type?.MakeGenericType(contract.Type.GenericTypeArguments);
            
            var local = parent.CreateContext(this, ref contract, manager);

            // If build is required send through build chain
            if (manager.RequireBuild) return ResolveRegistration(ref local);

            // Create mapping if nothing to build
            if (contract.Type != manager.Type)
            {
                var closure = new Contract(manager.Type!, contract.Name);
                manager.Pipeline = (ref PipelineContext c) =>
                {
                    var contract = closure;
                    return c.Container.Resolve(ref contract, ref c);
                };

                return ResolveRegistration(ref local);
            }

            // Check if factory exists
            if (_policies.TryGet(definition, out ResolveDelegate<PipelineContext>? pipeline))
            {
                manager.Pipeline = pipeline;
            }
            else if (_policies.TryGet(definition, out PipelineFactory? factory))
            {
                manager.Pipeline = factory!(contract.Type);
            }

            return ResolveRegistration(ref local);

        }


        private object? ResolveUnregisteredGeneric(ref Contract generic, ref PipelineContext context)
        {
            if (!_policies.TryGet(context.Contract.Type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (!_policies.TryGet(generic.Type, out PipelineFactory? factory))
                    return ResolveUnregistered(ref context);

                pipeline = factory!(context.Contract.Type);
            }

            return pipeline!(ref context);
        }
    }
}
