using System;
using System.Collections.Generic;
using Unity.Container;
using Unity.Storage;

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

            // Type Chain
            context.TypePipelineChain.Add(UnityBuildStage.Creation,     new ConstructorProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Fields,       new FieldProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Properties,   new PropertyProcessor(context.Policies));
            context.TypePipelineChain.Add(UnityBuildStage.Methods,      new MethodProcessor(context.Policies));
            
            // Factory Chain
            context.FactoryPipelineChain.Add(UnityBuildStage.Creation,  new FactoryProcessor());
            
            // Instance Chain
            context.InstancePipelineChain.Add(UnityBuildStage.Creation, new InstanceProcessor());

            
            // Populate pipelines

            context.Policies.Set<Policies.CategoryType, ResolveDelegate<PipelineContext>>(
                BuilderStrategy.BuildUp<PipelineContext>(context.TypePipelineChain));

            context.Policies.Set<Policies.CategoryFactory, ResolveDelegate<PipelineContext>>(
                BuilderStrategy.BuildUp<PipelineContext>(context.FactoryPipelineChain));

            context.Policies.Set<Policies.CategoryInstance, ResolveDelegate<PipelineContext>>(
                BuilderStrategy.BuildUp<PipelineContext>(context.InstancePipelineChain));

            
            // Rebuild when changed

            ((IStagedStrategyChain)context.TypePipelineChain).Invalidated     += OnBuildChainChanged;
            ((IStagedStrategyChain)context.FactoryPipelineChain).Invalidated  += OnBuildChainChanged;
            ((IStagedStrategyChain)context.InstancePipelineChain).Invalidated += OnBuildChainChanged;

            void OnBuildChainChanged(object chain, Type target) 
                => context.Policies.Set<ResolveDelegate<PipelineContext>>(target, 
                    BuilderStrategy.BuildUp<PipelineContext>((IEnumerable<BuilderStrategy>)chain));
        }
    }
}
