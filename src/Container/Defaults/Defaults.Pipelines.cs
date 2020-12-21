using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Resolution;
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
            => (PipelineFactory<PipelineContext>)Data[PIPELINE_FACTORY_TYPE].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ContextualFactory<PipelineContext> ContextualFactory
            => (ContextualFactory<PipelineContext>)Data[PIPELINE_FACTORY_CONTEXT].Value!;

        #endregion


        #region Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> TypePipeline
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_TYPE].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> InstancePipeline
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_INSTANCE].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> FactoryPipeline
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_FACTORY].Value!;

        #endregion


        #region Arrays and Enumerable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<Scope, Type[], Metadata[]> MetaArray
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ARRAY].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<Scope, Type[], Metadata[]> MetaEnumeration
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ENUMERATION].Value!;

        public Func<UnityContainer, Type, Type> GerTargetType
            => (Func<UnityContainer, Type, Type>)Data[TO_ARRAY_TYPE].Value!;

        #endregion
    }
}
