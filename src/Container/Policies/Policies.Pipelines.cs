using System.Diagnostics;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class Policies<TContext>
    {
        #region Algorithms

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> ResolveRegistered { get; private set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> ResolveUnregistered { get; private set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> ResolveArray { get; private set; }
            = UnityContainer.DummyPipeline;

        #endregion


        #region Pipelines

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> ActivatePipeline { get; internal set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> InstancePipeline { get; internal set; }
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> FactoryPipeline { get; internal set; } 
            = UnityContainer.DummyPipeline;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ResolveDelegate<TContext> MappingPipeline { get; internal set; }
            = UnityContainer.DummyPipeline;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PipelineFactory<TContext> PipelineFactory { get; private set; } 
            = (ref TContext context) => UnityContainer.DummyPipeline;

        #endregion
    }
}
