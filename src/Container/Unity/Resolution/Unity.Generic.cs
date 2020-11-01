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

            if (!manager.RequireBuild && context.Contract.Type != manager.Type)
            {
                var contract = new Contract(manager.Type!, context.Contract.Name);

                manager.Pipeline = (ref PipelineContext c) =>
                {
                    var stack = contract;
                    var local = c.CreateContext(ref stack);

                    c.Target = local.Resolve();

                    return c.Target;
                };
            }

            ResolveRegistration(ref context);

            return context.Target;
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
