using System;
using System.ComponentModel.Composition;
using System.Reflection;
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
            _policies.Set<FromTypeFactory<PipelineContext>>(FromTypeResolved);
            _policies.Set<PipelineFactory<PipelineContext>>(PipelineFromContext);

            // Default 'Chain Execution Pipeline' factory
            _policies.Set<Func<IStagedStrategyChain, ResolveDelegate<PipelineContext>>>(
                PipelineBuilder<PipelineContext>.BuildUp);
        }


        #region Default Pipeline Factories

        #region From Context

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

                    return _policies!.ActivatePipeline;

                case RegistrationCategory.Factory:
                    return _policies!.FactoryPipeline;

                case RegistrationCategory.Instance:
                    return _policies!.InstancePipeline;

                default:
                    return FromTypeResolved(context.Type);
            }
        }

        #endregion


        #region From Type

        internal static ResolveDelegate<PipelineContext> FromTypeActivated(Type type) 
            => _policies!.ActivatePipeline;

        internal static ResolveDelegate<PipelineContext> FromTypeResolved(Type type)
        {
            var builder = new PipelineBuilder<PipelineContext>(_policies!.TypeChain.ToArray());

            var pipeline = _policies!.ActivatePipeline;

            
            return pipeline;
        }

        internal static ResolveDelegate<PipelineContext> FromTypeCompiled(Type type)
        {
            var pipeline = _policies!.ActivatePipeline;


            return pipeline;
        }

        private static ResolveDelegate<PipelineContext> CompositionPipeline(Type type)
        {
            var policy = type.GetCustomAttribute<PartCreationPolicyAttribute>();
            var pipeline = null != policy && CreationPolicy.Shared == policy.CreationPolicy
                         ? (ref PipelineContext context) =>
                         {
                             // TODO: Optimize ??
                             var manager = context.Container.Root.Scope.GetCache(in context.Contract, () => new ContainerControlledLifetimeManager());
                             lock (manager)
                             {
                                 context.Registration = manager;

                                 manager.Category = RegistrationCategory.Type;
                                 var pipeline = manager.SetPipeline(context.Container.Scope, _policies!.ActivatePipeline)!;

                                 return pipeline(ref context);
                             }
                         }
            : _policies!.ActivatePipeline;

            _policies!.Set<ResolveDelegate<PipelineContext>>(type, pipeline);

            return pipeline;
        }

        #endregion

        #endregion
    }
}
