using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Pipeline Chains

        public StagedChain<UnityBuildStage, BuilderStrategy> TypeChain { get; }

        public StagedChain<UnityBuildStage, BuilderStrategy> FactoryChain { get; }

        public StagedChain<UnityBuildStage, BuilderStrategy> InstanceChain { get; }

        #endregion


        #region Pipeline Factories

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory<PipelineContext> PipelineFactory { get; private set; } 
            = (ref PipelineContext context) => DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverFactory<PipelineContext> ResolverFactory { get; private set; } 
            = (Type type) => DummyPipeline;

        #endregion


        #region Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> TypePipeline { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> InstancePipeline { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> FactoryPipeline { get; private set; } 
            = DummyPipeline;


        #endregion


        #region Arrays and Enumerable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public UnitySelector<Type, Type> ArrayTargetType { get; private set; } 
            = (c, i) => throw new NotImplementedException();

        #endregion
    }
}
