using System.Collections.Generic;
using Unity.Container;
using Unity.Extension;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public static class BuiltInComponents
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
            ((StagedChain<UnityBuildStage, PipelineProcessor>)context.TypePipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Creation,   constructor),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Fields,     field),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Properties, property),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Methods,    method));

            // Initialize Factory Chain
            ((StagedChain<UnityBuildStage, PipelineProcessor>)context.FactoryPipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Creation,    factory));

            // Initialize Instance Chain
            ((StagedChain<UnityBuildStage, PipelineProcessor>)context.InstancePipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Creation,    instance));

            // Initialize Unregistered Chain
            ((StagedChain<UnityBuildStage, PipelineProcessor>)context.UnregisteredPipelineChain)
                .Add(new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Creation,   constructor),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Fields,     field),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Properties, property),
                     new KeyValuePair<UnityBuildStage, PipelineProcessor>(UnityBuildStage.Methods,    method));

            #endregion


            #region Factories

            LazyFactory.Setup(context);
            FuncFactory.Setup(context);
            DefaultPipelineFactory.Setup(context);

            #endregion
        }
    }
}
