using System;
using System.Diagnostics;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
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


        #region Metadata Recorders

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<Scope, Type[], Metadata[]> MetaArray
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ARRAY].Value!;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<Scope, Type[], Metadata[]> MetaEnumeration
            => (Func<Scope, Type[], Metadata[]>)Data[TO_ENUMERATION].Value!;

        #endregion
    }
}
