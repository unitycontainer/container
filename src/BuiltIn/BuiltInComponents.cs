using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public static class BuiltInComponents
    {
        #region Fields

        private static MethodInfo Activator = 
            typeof(PipelineProcessor)
#if NETSTANDARD1_0 || NETSTANDARD1_6
            .GetTypeInfo()
#endif
            .GetMethod(nameof(PipelineProcessor.DefaultResolver))!;

        #endregion

        public static void Setup(ExtensionContext context)
        {
            // Default policies
            var defaults = (Defaults)context.Policies;

            // Subscribe to notifications
            //defaults.TypeChain.ChainChanged += (chain) => 
            //    defaults.TypeActivationPipeline = (ResolveDelegate<ResolveContext>)
            //        Delegate.CreateDelegate(typeof(ResolveDelegate<ResolveContext>), chain, Activator);

            //defaults.FactoryChain.ChainChanged += (chain) =>
            //    defaults.FactoryActivationPipeline = (ResolveDelegate<ResolveContext>)
            //        Delegate.CreateDelegate(typeof(ResolveDelegate<ResolveContext>), chain, Activator);

            //defaults.InstanceChain.ChainChanged += (chain) =>
            //    defaults.InstanceActivationPipeline = (ResolveDelegate<ResolveContext>)
            //        Delegate.CreateDelegate(typeof(ResolveDelegate<ResolveContext>), chain, Activator);

            //defaults.UnregisteredChain.ChainChanged += (chain) =>
            //    defaults.UnregisteredActivationPipeline = (ResolveDelegate<ResolveContext>)
            //        Delegate.CreateDelegate(typeof(ResolveDelegate<ResolveContext>), chain, Activator);


            // Create processors
            var field       = new FieldProcessor(defaults);
            var method      = new MethodProcessor(defaults);
            var factory     = new FactoryProcessor(defaults);
            var lifetime    = new LifetimeProcessor(defaults);
            var property    = new PropertyProcessor(defaults);
            var instance    = new InstanceProcessor(defaults);
            var constructor = new ConstructorProcessor(defaults);


            // Initialize Type Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.TypePipelineChain)
                .Add((BuilderStage.Creation,   constructor),
                     (BuilderStage.Fields,     field),
                     (BuilderStage.Properties, property),
                     (BuilderStage.Methods,    method), 
                     (BuilderStage.Lifetime,   lifetime));

            // Initialize Factory Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.FactoryPipelineChain)
                .Add((BuilderStage.Factory,    factory), 
                     (BuilderStage.Lifetime,   lifetime));

            // Initialize Instance Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.InstancePipelineChain)
                .Add((BuilderStage.Factory,    instance),
                     (BuilderStage.Lifetime,   lifetime));

            // Initialize Unregistered Chain
            ((StagedChain<BuilderStage, PipelineProcessor>)context.UnregisteredPipelineChain)
                .Add((BuilderStage.Creation,   constructor),
                     (BuilderStage.Fields,     field),
                     (BuilderStage.Properties, property),
                     (BuilderStage.Methods,    method));
        }
    }
}
