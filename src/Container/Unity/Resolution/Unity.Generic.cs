using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Initialize newly create Bound Generic registration
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> to build</param>
        /// <param name="definition">Generic <see cref="Type"/> definition</param>
        /// <param name="manager">New <see cref="RegistrationManager"/> created in previous step</param>
        /// <param name="parent"><see cref="PipelineContext"/> of last step</param>
        /// <returns>Resolved value</returns>
        private object? GenericRegistration(Type definition, ref PipelineContext context)
        {
            // Unbound generic manager
            var manager = context.Registration!;
            var unbound = (RegistrationManager)manager!.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = unbound.Type?.MakeGenericType(context.Contract.Type.GenericTypeArguments);

            if (!manager.RequireBuild)
            { 
                if (context.Contract.Type != manager.Type)
                {
                    // Create mapping if nothing to build
                    var closure = new Contract(manager.Type!, context.Contract.Name);
                    manager.Pipeline = (ref PipelineContext c) =>
                    {
                        var contract = closure;
                        return c.Container.ResolveContract(ref contract, ref c);
                    };
                } 
                else if (_policies.TryGet(definition, out PipelineFactory? factory))
                { 
                    // Build from a factory
                    manager.Pipeline = factory!(context.Contract.Type);
                }
            }

            return ResolveRegistration(ref context);
        }

        // TODO: cover missing cases
        private object? GenericUnregistered(ref Contract generic, ref PipelineContext context)
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
