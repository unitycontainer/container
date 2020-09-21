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
            var lifetime    = new LifetimeProcessor(defaults);
            var property    = new PropertyProcessor(defaults);
            var instance    = new InstanceProcessor(defaults);
            var constructor = new ConstructorProcessor(defaults);

            #endregion


            #region Chains

            // Initialize Type Chain
            ((StagedChain<BuildStage, PipelineProcessor>)context.TypePipelineChain)
                .Add(new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Lifetime,   lifetime),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Creation,   constructor),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Fields,     field),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Properties, property),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Methods,    method));

            // Initialize Factory Chain
            ((StagedChain<BuildStage, PipelineProcessor>)context.FactoryPipelineChain)
                .Add(new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Lifetime,   lifetime),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Factory,    factory),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Fields,     field),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Properties, property),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Methods,    method));

            // Initialize Instance Chain
            ((StagedChain<BuildStage, PipelineProcessor>)context.InstancePipelineChain)
                .Add(new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Lifetime,   lifetime), 
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Factory,    instance),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Fields,     field),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Properties, property),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Methods,    method));

            // Initialize Unregistered Chain
            ((StagedChain<BuildStage, PipelineProcessor>)context.UnregisteredPipelineChain)
                .Add(new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Creation,   constructor),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Fields,     field),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Properties, property),
                     new KeyValuePair<BuildStage, PipelineProcessor>(BuildStage.Methods,    method));

            #endregion


            #region Pipelines

            DefaultPipelineFactory.Setup(context);
            BalancedPipelineFactory.Setup(context);
            OptimizedPipelineFactory.Setup(context);
            SingletonPipelineFactory.Setup(context);
            UnregisteredPipelineFactory.Setup(context);

            #endregion
        }
    }
}
