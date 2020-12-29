using System;
using System.Collections.Generic;
using Unity.Container;

namespace Unity.Extension
{
    /// <summary>
    /// This extension installs the default strategies and policies into the container
    /// to implement the standard behavior of the Unity container.
    /// </summary>
    public partial class UnityDefaultStrategiesExtension
    {
        /// <summary>
        /// Add the default <see cref="BuilderStrategy"/> strategies and policies to the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Build Stages
            var policies = context.Policies;

            // Type Chain
            context.TypePipelineChain.Add(UnityBuildStage.Fields,       new FieldProcessor(policies));
            context.TypePipelineChain.Add(UnityBuildStage.Methods,      new MethodProcessor(policies));
            context.TypePipelineChain.Add(UnityBuildStage.Creation,     new ConstructorProcessor(policies));
            context.TypePipelineChain.Add(UnityBuildStage.Properties,   new PropertyProcessor(policies));
            
            // Factory Chain
            context.FactoryPipelineChain.Add(UnityBuildStage.Creation,  new FactoryProcessor());
            
            // Instance Chain
            context.InstancePipelineChain.Add(UnityBuildStage.Creation, new InstanceProcessor());


            // Populate pipelines

            policies.Set<Policies.CategoryType, ResolveDelegate<PipelineContext>>(
                PipelineBuilder<PipelineContext>.CompileBuildUp(context.TypePipelineChain));

            policies.Set<Policies.CategoryFactory, ResolveDelegate<PipelineContext>>(
                PipelineBuilder<PipelineContext>.CompileBuildUp(context.FactoryPipelineChain));

            policies.Set<Policies.CategoryInstance, ResolveDelegate<PipelineContext>>(
                PipelineBuilder<PipelineContext>.CompileBuildUp(context.InstancePipelineChain));


            // Rebuild when changed
            context.TypePipelineChain.Invalidated     += OnBuildChainChanged;
            context.FactoryPipelineChain.Invalidated  += OnBuildChainChanged;
            context.InstancePipelineChain.Invalidated += OnBuildChainChanged;

            void OnBuildChainChanged(IStagedStrategyChain chain, Type target) 
                => policies.Set<ResolveDelegate<PipelineContext>>(target,
                    PipelineBuilder<PipelineContext>.CompileBuildUp(chain));
        }
    }
}
