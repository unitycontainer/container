using System;
using Unity.Container;
using Unity.Extension;

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
            switch (unbound.Category)
            {
                case RegistrationCategory.Type:
                    
                    try
                    {
                        manager.Category = RegistrationCategory.Type;
                        manager.Data = unbound.Type?.MakeGenericType(context.Contract.Type.GenericTypeArguments);
                    }
                    catch (Exception ex) 
                    {
                        // If type could not be created, it will throw an exception
                        return context.Throw(ex);
                    }
                    
                    if (!manager.RequireBuild)
                    {
                        if (context.Contract.Type != manager.Type)
                        {
                            // Create mapping if nothing to build
                            var closure = new Contract(manager.Type!, context.Contract.Name);
                            manager.Pipeline = (ref PipelineContext c) =>
                            {
                                var contract = closure;
                                var map = c.CreateMap(ref contract);
                                return c.Container.Resolve(ref map);
                            };
                        }
                        else if (Policies.TryGet(definition, out ResolverFactory<PipelineContext>? factory))
                        {
                            // Build from a factory
                            manager.Pipeline = factory!(context.Contract.Type);
                        }
                    }
                    break;

                case RegistrationCategory.Factory:
                    manager.Category = RegistrationCategory.Factory;
                    manager.Data = unbound.Data;
                    break;

                default:
                    throw new NotSupportedException();
            }

            // Lock and resolve
            manager.GetValue(context.Container.Scope);
            return ResolveRegistered(ref context);
        }

        // TODO: cover missing cases
        private object? GenericUnregistered(ref Contract generic, ref PipelineContext context)
        {
            if (!Policies.TryGet(context.Contract.Type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (!Policies.TryGet(generic.Type, out ResolverFactory<PipelineContext>? factory))
                    return ResolveUnregistered(ref context);

                pipeline = factory!(context.Contract.Type);
            }

            return pipeline!(ref context);
        }
    }
}
