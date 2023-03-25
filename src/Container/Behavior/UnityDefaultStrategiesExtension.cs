using System;
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
            var policies = context.Policies;


            // Populate activation stages
            //policies.Set<Policies<BuilderContext>.IStrategiesChain, Func<IStagedStrategyChain<BuilderStrategy, UnityBuildStage>>>(
            //    () => new StagedStrategyChain<BuilderStrategy, UnityBuildStage>
            //    {
            //        (UnityBuildStage.Fields,     new FieldStrategy(policies)),
            //        (UnityBuildStage.Methods,    new MethodStrategy(policies)),
            //        (UnityBuildStage.Creation,   new ConstructorStrategy(policies)),
            //        (UnityBuildStage.Properties, new PropertyStrategy(policies))
            //    });

            context.Strategies.Add((UnityBuildStage.Fields,      new FieldStrategy(policies)),
                                    (UnityBuildStage.Methods,    new MethodStrategy(policies)),
                                    (UnityBuildStage.Creation,   new ConstructorStrategy(policies)),
                                    (UnityBuildStage.Properties, new PropertyStrategy(policies)));

            // Factory chain initializer
            policies.Set<Policies<BuilderContext>.IFactoryChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>
                {
                    (UnityBuildStage.Creation, FactoryStrategy<BuilderContext>.BuilderStrategyDelegate)
                });

            // Mapping chain initializer
            policies.Set<Policies<BuilderContext>.IMappingChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>
                { 
                    (UnityBuildStage.TypeMapping, MappingStrategy<BuilderContext>.BuilderStrategyDelegate) 
                });

            // Instance chain initializer
            policies.Set<Policies<BuilderContext>.IInstanceChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>>>(
                () => new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>
                { 
                    (UnityBuildStage.Creation, InstanceStrategy<BuilderContext>.BuilderStrategyDelegate) 
                });
        }
    }
}
