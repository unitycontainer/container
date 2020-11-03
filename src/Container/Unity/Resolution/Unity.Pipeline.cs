using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<PipelineContext> BuildPipeline(ref PipelineContext context)
        {
            switch (context.Registration?.Category)
            {
                case RegistrationCategory.Type:

                    // Check for Type Mapping
                    if (!context.Registration.RequireBuild && context.Contract.Type != context.Registration.Type)
                    {
                        var closure = new Contract(context.Registration.Type!, context.Contract.Name);
                        // Mapping resolver
                        return (ref PipelineContext c) =>
                        {
                            var contract = closure;
                            return c.Container.Resolve(ref contract, ref c);
                        };
                    }
                    
                    return _policies.BuildTypePipeline(context.Contract.Type);

                case RegistrationCategory.Factory:
                    return _policies.BuildFactoryPipeline(context.Contract.Type);

                case RegistrationCategory.Instance:
                    return _policies.BuildInstancePipeline(context.Contract.Type);

                default:
                    throw new NotImplementedException();
            }
        }

        private ResolveDelegate<PipelineContext> BuildPipelineUnregistered(Type type)
        {
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            if (null != policy && CreationPolicy.Shared == policy.CreationPolicy)
            {
                return (ref PipelineContext context) =>
                {
                    var manager = Root._scope.GetCache(in context.Contract, new ContainerControlledLifetimeManager());
                    lock (manager)
                    {
                        context.Registration = manager;

                        manager.Category = RegistrationCategory.Type;
                        manager.Pipeline = _policies.BuildTypePipeline(type);

                        return manager.Pipeline(ref context);
                    }
                };
            }

            return _policies.BuildTypePipeline(type);
        }
    }
}
