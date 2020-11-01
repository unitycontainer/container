using System;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Chains

        public StagedChain<BuildStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Factories

        public PipelineFactory<PipelineContext> BuildPipeline
            => (PipelineFactory<PipelineContext>)Data[BUILD_PIPELINE].Value!;

        public PipelineFactory<PipelineContext> BuildTypePipeline 
            => (PipelineFactory<PipelineContext>)Data[BUILD_PIPELINE_TYPE].Value!;

        public PipelineFactory<PipelineContext> BuildInstancePipeline 
            => (PipelineFactory<PipelineContext>)Data[BUILD_PIPELINE_INSTANCE].Value!;

        public PipelineFactory<PipelineContext> BuildFactoryPipeline 
            => (PipelineFactory<PipelineContext>)Data[BUILD_PIPELINE_FACTORY].Value!;

        #endregion


        #region Metadata Recorders

        public Func<Scope, Type[], Metadata[]> MetaArray
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ARRAY].Value!;

        public Func<Scope, Type[], Metadata[]> MetaEnumeration
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ENUMERATION].Value!;

        #endregion
    }
}
