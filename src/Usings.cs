global using System;
global using System.Linq;

#region Pipeline Types

global using IBuildPlanChain = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityBuildStage>;
global using IActivateChain = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityActivateStage>;
global using IFactoryChain = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityFactoryStage>;
global using IInstanceChain = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityInstanceStage>;
global using IMappingChain = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityMappingStage>;

global using ChainToFactoryConverter = System.Converter<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>[], Unity.Resolution.PipelineFactory<Unity.Builder.BuilderContext>>;
global using ChainToResolverConverter = System.Converter<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>[], Unity.Resolution.ResolveDelegate<Unity.Builder.BuilderContext>>;

global using FactoryPipeline  = Unity.Resolution.PipelineFactory<Unity.Builder.BuilderContext>;
global using ResolverPipeline = Unity.Resolution.ResolveDelegate<Unity.Builder.BuilderContext>;

#endregion
