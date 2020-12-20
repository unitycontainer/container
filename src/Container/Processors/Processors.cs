using System.Collections.Generic;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    internal static class Processors
    {
        public static void Setup(ExtensionContext context)
        {
            // Default policies
            var defaults = (Defaults)context.Policies;

            #region Processors

            // Create processors
            var field       = new FieldProcessor(defaults);
            var method      = new MethodProcessor(defaults);
            var factory     = new FactoryProcessor(defaults);
            var property    = new PropertyProcessor(defaults);
            var instance    = new InstanceProcessor(defaults);
            var constructor = new ConstructorProcessor(defaults);

            #endregion


            #region Chains

            // Initialize Type Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.TypePipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,   constructor),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Fields,     field),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Properties, property),
                     new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Methods,    method));

            // Initialize Factory Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.FactoryPipelineChain)
                .Add(UnityBuildStage.Creation,    factory);

            // Initialize Instance Chain
            ((StagedChain<UnityBuildStage, BuilderStrategy>)context.InstancePipelineChain)
                .Add(UnityBuildStage.Creation,    instance);

            #endregion
        }
    }
}
