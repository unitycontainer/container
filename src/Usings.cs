global using System;
global using System.Linq;

#region Pipeline Types

global using IBuildPlanChain  = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityBuildStage>;
global using IActivateChain   = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityActivateStage>;
global using IFactoryChain    = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityFactoryStage>;
global using IInstanceChain   = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityInstanceStage>;
global using IMappingChain    = Unity.Storage.IStagedStrategyChain<Unity.Strategies.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityMappingStage>;

#endregion
