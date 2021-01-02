using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class Pipelines
    {
        #region Fields

        private static IEnumerable<BuilderStrategy> _typeChain;

        // TODO: Replace with individual policies
        private static Policies? _policies;

        private static ResolveDelegate<PipelineContext> ActivatePipeline
            = UnityContainer.DummyPipeline;

        private static ResolveDelegate<PipelineContext> FactoryPipeline 
            = UnityContainer.DummyPipeline;

        private static ResolveDelegate<PipelineContext> InstancePipeline
            = UnityContainer.DummyPipeline;

        #endregion


        public static void Setup(ExtensionContext context)
        {
            _policies = (Policies)context.Policies;

            // Pipeline Factories
            _policies.Set<PipelineFactory<PipelineContext>>(PipelineFactory);
            _policies.Set<PipelineFactory<PipelineContext>>(typeof(Type), FromTypeFactory);

            // Default 'Chain Execution Pipeline' factory
            _policies.Set<Func<IStagedStrategyChain, ResolveDelegate<PipelineContext>>>(
                PipelineBuilder<PipelineContext>.BuildUp);

            // TODO: Requires optimization
            ActivatePipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
                typeof(Activator), OnActivatePipelineChanged)!;

            FactoryPipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
                typeof(IUnityContainer.FactoryDelegate), OnFactoryPipelineChanged)!;

            InstancePipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
                typeof(Policies.CategoryInstance), OnInstancePipelineChanged)!;
        }



        public static ResolveDelegate<PipelineContext> PipelineFactory(ref PipelineContext context)
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

                    return FromTypeFactory(ref context);

                case RegistrationCategory.Factory:
                    return FactoryPipeline;

                case RegistrationCategory.Instance:
                    return InstancePipeline;

                default:
                    return FromTypeFactory(ref context);
            }
        }


        public static ResolveDelegate<PipelineContext> FromTypeFactory(ref PipelineContext context)
        {
            switch (context.Registration?.CreationPolicy)
            {
                case CreationPolicy.Any:
                    break;
                    
                case CreationPolicy.Shared:
                    return ActivatePipeline;
                
                case CreationPolicy.NonShared:
                    break;
            }

            //return  ActivatePipeline;
            var builder = new PipelineBuilder<PipelineContext>(_policies!.TypeChain);

            return builder.Build() ?? UnityContainer.DummyPipeline; 
        }


        #region Chains Changes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnActivatePipelineChanged(Type? target, Type type, object? policy)
            => ActivatePipeline = (ResolveDelegate<PipelineContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnFactoryPipelineChanged(Type? target, Type type, object? policy)
            => FactoryPipeline = (ResolveDelegate<PipelineContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnInstancePipelineChanged(Type? target, Type type, object? policy)
            => InstancePipeline = (ResolveDelegate<PipelineContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
