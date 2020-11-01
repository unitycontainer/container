using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<PipelineContext> BuildPipelineUnregistered(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            if (null != policy && CreationPolicy.Shared == policy.CreationPolicy)
            {
                // TODO: First naive implementation. Requires verification
                var manager = Root._scope.GetCache(in context.Contract, new ContainerControlledLifetimeManager());
                lock (manager)
                {
                    context.Registration = manager;
                    context.Registration.Pipeline = _policies.BuildTypePipeline(ref context);
                    
                    return context.Registration!.Pipeline;
                }
            }

            return _policies.BuildTypePipeline(ref context);
        }
    }
}
