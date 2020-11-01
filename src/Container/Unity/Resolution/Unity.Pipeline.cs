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
