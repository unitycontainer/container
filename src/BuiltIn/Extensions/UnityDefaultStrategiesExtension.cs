using System;
using Unity.Container;
using Unity.Resolution;

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
            var policies = context.Policies;

            // Setup build on change for the chains
            context.TypePipelineChain.Invalidated     += OnBuildChainChanged;
            context.FactoryPipelineChain.Invalidated  += OnBuildChainChanged;
            context.InstancePipelineChain.Invalidated += OnBuildChainChanged;


            // Populate Stages

            // Type Build Stages
            context.TypePipelineChain.Add(new (UnityBuildStage, BuilderStrategy)[] 
            { 
                (UnityBuildStage.Fields, new FieldProcessor(policies)),
                (UnityBuildStage.Methods, new MethodProcessor(policies)),
                (UnityBuildStage.Creation, new ConstructorProcessor(policies)),
                (UnityBuildStage.Properties, new PropertyProcessor(policies))
            });

            // Factory Build Stages
            context.FactoryPipelineChain.Add(UnityBuildStage.Creation,  new FactoryStrategy());

            // Instance Build Stages
            context.InstancePipelineChain.Add(UnityBuildStage.Creation, new InstanceProcessor());


            
            // Rebuilds stages chains when modified
            void OnBuildChainChanged(IStagedStrategyChain chain, Type target)
            {
                // Get 'Chain Execution Pipeline' factory
                var factory = policies.Get<Func<IStagedStrategyChain, ResolveDelegate<PipelineContext>>>()
                    ?? throw new ArgumentNullException("Invalid Factory");

                // Build Execution Pipeline and save
                policies.Set<ResolveDelegate<PipelineContext>>(target, factory(chain));
            }
        }
    }
}
