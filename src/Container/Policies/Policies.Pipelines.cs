using System;
using System.Diagnostics;
using Unity.Extension;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Pipeline Factories

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory<PipelineContext> PipelineFactory { get; private set; }
            = (ref PipelineContext context) => DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public FromTypeFactory<PipelineContext> FromTypeFactory { get; private set; } 
            = (Type type) => DummyPipeline;

        #endregion


        #region Algorithms

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ResolveRegistered { get; private set; }
            = DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<PipelineContext> ResolveUnregistered { get; private set; }
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


        #region Arrays and Enumerable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SelectorDelegate<Type, Type> ArrayTargetType { get; private set; } 
            = (c, i) => throw new NotImplementedException();

        #endregion
    }
}
