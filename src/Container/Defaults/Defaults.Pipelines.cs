using System;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Chains

        public StagedChain<UnityBuildStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<UnityBuildStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<UnityBuildStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<UnityBuildStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Factories

        public PipelineFactory BuildPipeline
            => (PipelineFactory)Data[BUILD_PIPELINE].Value!;

        public PipelineFactory BuildTypePipeline 
            => (PipelineFactory)Data[BUILD_PIPELINE_TYPE].Value!;

        public PipelineFactory BuildInstancePipeline 
            => (PipelineFactory)Data[BUILD_PIPELINE_INSTANCE].Value!;

        public PipelineFactory BuildFactoryPipeline 
            => (PipelineFactory)Data[BUILD_PIPELINE_FACTORY].Value!;

        #endregion


        #region Metadata Recorders

        public Func<Scope, Type[], Metadata[]> MetaArray
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ARRAY].Value!;

        public Func<Scope, Type[], Metadata[]> MetaEnumeration
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ENUMERATION].Value!;

        #endregion
    }
}
