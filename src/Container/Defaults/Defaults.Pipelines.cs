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
        public PipelineFactory<PipelineContext> PipelineFactory
            => (PipelineFactory<PipelineContext>)Data[PIPELINE_FACTORY].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverFactory<PipelineContext> ResolverFactory
            => (ResolverFactory<PipelineContext>)Data[RESOLVER_FACTORY].Value!;

        #endregion


        #region Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> TypePipeline
            => (ResolveDelegate<PipelineContext>)Data[BUILD_PIPELINE_TYPE].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> InstancePipeline
            => (ResolveDelegate<PipelineContext>)Data[BUILD_PIPELINE_INSTANCE].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> FactoryPipeline
            => (ResolveDelegate<PipelineContext>)Data[BUILD_PIPELINE_FACTORY].Value!;

        #endregion


        #region Arrays and Enumerable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public UnitySelector<Type, Type> ArrayTargetType
            => (UnitySelector<Type, Type>)Data[GET_TARGET_TYPE].Value!;

        #endregion
    }
}
