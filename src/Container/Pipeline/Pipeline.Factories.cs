using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        #region Fields
        
        // TODO: Replace with individual policies
        private static Policies? _policies;

        #endregion


        public static void PipelineFactories(ExtensionContext context)
        {
            _policies = (Policies)context.Policies;
            
            // Pipeline Factories
            context.Policies.Set<FromTypeFactory<PipelineContext>>(PipelineFromType);
            context.Policies.Set<PipelineFactory<PipelineContext>>(PipelineFromContext);
        }


        #region Default Pipeline Factories

        private static ResolveDelegate<PipelineContext> PipelineFromContext(ref PipelineContext context)
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
                            var map = c.CreateMap(ref contract);
                            return c.Container.Resolve(ref map);
                        };
                    }

                    return _policies!.TypePipeline;

                case RegistrationCategory.Factory:
                    return _policies!.FactoryPipeline;

                case RegistrationCategory.Instance:
                    return _policies!.InstancePipeline;

                default:
                    return PipelineFromType(context.Type);
            }
        }

        private static ResolveDelegate<PipelineContext> PipelineFromType(Type type)
        {
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            var pipeline = _policies!.TypePipeline;

            _policies!.Set<ResolveDelegate<PipelineContext>>(type, pipeline);
            
            return pipeline;
        }

        private static ResolveDelegate<PipelineContext> CompositionPipeline(Type type)
        {
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            var pipeline = null != policy && CreationPolicy.Shared == policy.CreationPolicy
                         ? (ref PipelineContext context) =>
                         {
                             // TODO: Optimize ??
                             var manager = context.Container.Root.Scope.GetCache(in context.Contract, new ContainerControlledLifetimeManager());
                             lock (manager)
                             {
                                 context.Registration = manager;

                                 manager.Category = RegistrationCategory.Type;
                                 manager.Pipeline = _policies!.TypePipeline;

                                 return manager.Pipeline(ref context);
                             }
                         }
            : _policies!.TypePipeline;

            _policies!.Set<ResolveDelegate<PipelineContext>>(type, pipeline);

            return pipeline;
        }

        #endregion
    }
}
