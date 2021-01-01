using System.Diagnostics;
using Unity.Extension;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Pipeline Factories

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory<PipelineContext> PipelineFactory { get; private set; }
            = DummyFactory;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public FromTypeFactory<PipelineContext> FromTypeFactory { get; private set; } 
            = DummyFactory;

        #endregion


        #region Algorithms

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ResolveRegistered { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ResolveUnregistered { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ResolveArray { get; private set; }
            = DummyPipeline;

        #endregion


        #region Chain Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ActivatePipeline { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> InstancePipeline { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> FactoryPipeline { get; private set; } 
            = DummyPipeline;


        #endregion
    }
}
