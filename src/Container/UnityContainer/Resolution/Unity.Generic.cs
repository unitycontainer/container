using Unity.Builder;
using Unity.Container;
using Unity.Extension;
using Unity.Policy;

namespace Unity
{
    public sealed partial class UnityContainer
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
                            manager.SetPipeline(((Policies)context.Policies).MappingPipeline);
                        }
                        else if (Policies.TryGet(definition, out FactoryPipeline? factory))
                        {
                            // Build from a factory
                            var pipeline = factory!(ref context);
                            manager.SetPipeline(pipeline);
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
            manager.GetValue(context.Container.LifetimeContainer);

            return Policies.ResolveRegistered(ref context);
        }

        /// <summary>
        /// Resolve unregistered generic type
        /// </summary>
        private object? UnregisteredGeneric(ref Contract generic, ref BuilderContext context)
        {
            if (!Policies.TryGet(generic.Type, out FactoryPipeline? factory))
                return ((Policies)context.Policies).ResolveUnregistered(ref context);

            var pipeline = factory!(ref context);
            
            Policies.Set<ResolverPipeline>(context.Type, pipeline);

            return pipeline!(ref context);
        }
    }
}
