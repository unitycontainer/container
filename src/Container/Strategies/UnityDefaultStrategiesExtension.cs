using System;
using Unity.Builder;
using Unity.Container;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Extension
{
    /// <summary>
    /// This extension installs the default strategies and policies into the container
    /// to implement the standard behavior of the Unity container.
    /// </summary>
    public partial class UnityDefaultStrategiesExtension<TContext>
            where TContext : IBuilderContext
    {
        /// <summary>
        /// Add the default <see cref="BuilderStrategy"/> strategies and policies to the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;


            // Populate Stages

            context.TypePipelineChain.Add((UnityBuildStage.Fields,     new FieldStrategy(policies)),
                                          (UnityBuildStage.Methods,    new MethodStrategy(policies)),
                                          (UnityBuildStage.Creation,   new ConstructorStrategy(policies)),
                                          (UnityBuildStage.Properties, new PropertyStrategy(policies)));

            context.FactoryPipelineChain.Add((UnityBuildStage.Creation,  new FactoryStrategy()));

            context.InstancePipelineChain.Add((UnityBuildStage.Creation,  new InstanceStrategy()));

            context.MappingPipelineChain.Add((UnityBuildStage.TypeMapping, new MappingStrategy()));
        }
    }
}
