using System;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Resolve registration just created from open generic registration
        /// </summary>
        private object? ResolveGeneric(Type definition, ref BuilderContext context)
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
                        // This is thrown when resolving collections and constraint
                        // is preventing proper instantiation.
                        return context.Throw(ex);
                    }
                    
                    if (!manager.RequireBuild)
                    {
                        if (context.Contract.Type != manager.Type)
                        {
                            var contract = new Contract(manager.Type!, context.Contract.Name);
                            
                            // Create mapping if nothing to build
                            manager.SetPipeline(context.Container.Scope, (ref BuilderContext c) => c.MapTo(contract));
                        }
                        else if (Policies.TryGet(definition, out PipelineFactory<BuilderContext>? factory))
                        {
                            // Build from a factory
                            manager.SetPipeline(context.Container.Scope, factory!(ref context));
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

            return Policies.ResolveRegistered(ref context);
        }

        /// <summary>
        /// Resolve unregistered generic type
        /// </summary>
        private object? UnregisteredGeneric(ref Contract generic, ref BuilderContext context)
        {
            if (!Policies.TryGet(generic.Type, out PipelineFactory<BuilderContext>? factory))
                return ((Policies<BuilderContext>)context.Policies).ResolveUnregistered(ref context);

            var pipeline = factory!(ref context);
            
            Policies.Set<ResolveDelegate<BuilderContext>>(context.Type, pipeline);

            return pipeline!(ref context);
        }
    }
}
