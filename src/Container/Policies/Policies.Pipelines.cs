using System.Diagnostics;
using Unity.Builder;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Algorithms

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline ResolveRegistered { get; private set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline ResolveUnregistered { get; private set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline ResolveArray { get; private set; }
            = UnityContainer.DummyPipeline;

        #endregion


        #region Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline ActivatePipeline { get; internal set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline InstancePipeline { get; internal set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline FactoryPipeline { get; internal set; } 
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolverPipeline MappingPipeline { get; internal set; }
            = UnityContainer.DummyPipeline;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public FactoryPipeline PipelineFactory { get; private set; } 
            = (ref BuilderContext context) => UnityContainer.DummyPipeline;

        #endregion
    }
}
