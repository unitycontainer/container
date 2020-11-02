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

        private object? GenericRegistration(ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            // Factory manager is in Data
            var factory = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = factory.Type?.MakeGenericType(contract.Type.GenericTypeArguments);

            // Create mapping if nothing to build
            if (!manager.RequireBuild && contract.Type != manager.Type)
            {
                var closure = new Contract(manager.Type!, contract.Name);

                manager.Pipeline = (ref PipelineContext c) =>
                {
                    var contract = closure;

                    return c.Container.Resolve(ref contract, ref c);
                };
            }

            var local = parent.CreateContext(this, ref contract, manager);

            ResolveRegistration(ref local);

            return local.Target;
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
