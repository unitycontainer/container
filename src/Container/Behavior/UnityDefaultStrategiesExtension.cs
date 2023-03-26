using Unity.Builder;
using Unity.Container;
using Unity.Storage;
using Unity.Strategies;

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
            var policies = (Policies<BuilderContext>)context.Policies;

            policies.ActivateChain.Add((UnityActivateStage.Creation,   new ConstructorStrategy(policies).PreBuildUp), 
                                       (UnityActivateStage.Fields,     new FieldStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Properties, new PropertyStrategy(policies).PreBuildUp),
                                       (UnityActivateStage.Methods,    new MethodStrategy(policies).PreBuildUp));

            // Factory chain initializer
            policies.FactoryPipeline  = FactoryStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IFactoryChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityFactoryStage>
                {
                    (UnityFactoryStage.Creation, FactoryStrategy<BuilderContext>.BuilderStrategyDelegate)
                });

            // Mapping chain initializer
            policies.MappingPipeline  = MappingStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IMappingChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityMappingStage>
                { 
                    (UnityMappingStage.TypeMapping, MappingStrategy<BuilderContext>.BuilderStrategyDelegate) 
                });

            // Instance chain initializer
            policies.InstancePipeline = InstanceStrategy<BuilderContext>.DefaultPipeline;
            policies.Set<Func<IInstanceChain>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityInstanceStage>
                { 
                    (UnityInstanceStage.Creation, InstanceStrategy<BuilderContext>.BuilderStrategyDelegate) 
                });
        }
    }
}
