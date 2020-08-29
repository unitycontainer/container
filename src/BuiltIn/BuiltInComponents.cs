using System.Collections.Generic;
using Unity.Container;
using Unity.Disgnostics;
using Unity.Extension;
using Unity.Pipeline;
using Unity.Resolution;
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


            #region Pipeline Chains

            // Initialize Type Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.TypePipelineChain)
                .Add(new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Creation,   constructor),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Fields,     field),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Properties, property),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Methods,    method), 
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Lifetime,   lifetime));

            // Initialize Factory Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.FactoryPipelineChain)
                .Add(new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Factory,    factory),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Lifetime,   lifetime));

            // Initialize Instance Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.InstancePipelineChain)
                .Add(new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Factory,    instance),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Lifetime,   lifetime));

            // Initialize Unregistered Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.UnregisteredPipelineChain)
                .Add(new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Creation,   constructor),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Fields,     field),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Properties, property),
                     new KeyValuePair<BuilderStage, PipelineProcessor>(BuilderStage.Methods,    method));

            #endregion


            #region Pipelines

            // Default activating pipelines
            defaults.Set(typeof(Defaults.TypeCategory),     
                         typeof(ResolveDelegate<ResolveContext>), 
                         (Telemetry.IsEnabled ? SingletonPipelineFactory.DiagnosticInfo : SingletonPipelineFactory.PipelineInfo)
                                                 .CreateDelegate(typeof(ResolveDelegate<ResolveContext>), 
                                                    ((StagedChain<BuilderStage, PipelineProcessor>)context.TypePipelineChain)
                                                    .ToArray()));
            
            defaults.Set(typeof(Defaults.FactoryCategory),  
                         typeof(ResolveDelegate<ResolveContext>),
                         (Telemetry.IsEnabled ? SingletonPipelineFactory.DiagnosticInfo : SingletonPipelineFactory.PipelineInfo)
                                                 .CreateDelegate(typeof(ResolveDelegate<ResolveContext>), 
                                                    ((StagedChain<BuilderStage, PipelineProcessor>)context.FactoryPipelineChain)
                                                    .ToArray()));
            
            defaults.Set(typeof(Defaults.InstanceCategory), 
                         typeof(ResolveDelegate<ResolveContext>),
                         (Telemetry.IsEnabled ? SingletonPipelineFactory.DiagnosticInfo : SingletonPipelineFactory.PipelineInfo)
                                                 .CreateDelegate(typeof(ResolveDelegate<ResolveContext>), 
                                                    ((StagedChain<BuilderStage, PipelineProcessor>)context.UnregisteredPipelineChain)
                                                    .ToArray()));
            // Install Pipeline Factories
            defaults.Set(typeof(ResolveDelegateFactory),               (ResolveDelegateFactory)DelegateFactory.Factory);
            defaults.Set(typeof(Defaults.BalancedFactoryDelegate),     (Defaults.BalancedFactoryDelegate)BalancedPipelineFactory.Factory);
            defaults.Set(typeof(Defaults.OptimizedFactoryDelegate),    (Defaults.OptimizedFactoryDelegate)OptimizedPipelineFactory.Factory);
            defaults.Set(typeof(Defaults.UnregisteredFactoryDelegate), (Defaults.UnregisteredFactoryDelegate)UnregisteredPipelineFactory.Factory);

            #endregion
        }
    }
}
