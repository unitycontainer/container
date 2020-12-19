using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Chains

        public StagedChain<UnityBuildStage, BuilderStrategy> TypeChain { get; }

        public StagedChain<UnityBuildStage, BuilderStrategy> FactoryChain { get; }

        public StagedChain<UnityBuildStage, BuilderStrategy> InstanceChain { get; }

        #endregion


        #region Factories

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory BuildPipeline
            => (PipelineFactory)Data[PIPELINE_BUILD].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory BuildTypePipeline 
            => (PipelineFactory)Data[PIPELINE_TYPE_BUILD].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory BuildInstancePipeline 
            => (PipelineFactory)Data[PIPELINE_INSTANCE_BUILD].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory BuildFactoryPipeline 
            => (PipelineFactory)Data[PIPELINE_FACTORY_BUILD].Value!;

        #endregion
    }
}
