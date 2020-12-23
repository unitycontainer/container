using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Default Pipeline Factories

        private ResolveDelegate<PipelineContext> BuildPipelineRegistered(ref PipelineContext context)
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

                    return Policies.TypePipeline;

                case RegistrationCategory.Factory:
                    return Policies.FactoryPipeline;

                case RegistrationCategory.Instance:
                    return Policies.InstancePipeline;

                default:
                    return (ref PipelineContext c)
                        => c.Error($"Invalid Registration Category: {c.Registration?.Category}");
            }
        }

        private ResolveDelegate<PipelineContext> BuildPipelineUnregistered(Type type)
        {
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            var pipeline = null != policy && CreationPolicy.Shared == policy.CreationPolicy
                         ? (ref PipelineContext context) =>
                         {
                             // TODO: Optimize ??
                             var manager = Root.Scope.GetCache(in context.Contract, new ContainerControlledLifetimeManager());
                             lock (manager)
                             {
                                 context.Registration = manager;

                                 manager.Category = RegistrationCategory.Type;
                                 manager.Pipeline = Policies.TypePipeline;

                                 return manager.Pipeline(ref context);
                             }
                         }
                         : Policies.TypePipeline;
            
            return Policies.GetOrAdd(type, pipeline);
        }

        #endregion


        #region Default Pipeline

        private void OnBuildChainChanged(StagedChain<UnityBuildStage, BuilderStrategy> chain)
            => Policies.Set<ResolveDelegate<PipelineContext>>(chain.Type, BuildUpPipelineFactory(chain));

        private static ResolveDelegate<PipelineContext> BuildUpPipelineFactory(IEnumerable<BuilderStrategy> chain)
        {
            var processors = chain.ToArray();
            return (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }

        #endregion
    }
}
