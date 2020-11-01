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

            ResolveRegistration(ref context);

            return context.Target;
        }


        private object? ResolveUnregisteredGeneric(ref Contract generic, ref PipelineContext context)
        {
            if (!_policies.TryGet(context.Contract.Type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                if (!_policies.TryGet(generic.Type, out PipelineFactory<Type>? factory))
                    return ResolveUnregistered(ref context);

                var type = context.Contract.Type;
                pipeline = factory!(ref type);
            }

            return pipeline!(ref context);
        }
    }
}
