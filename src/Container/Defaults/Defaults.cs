using System;
using System.Collections;
using System.Diagnostics;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Defaults
    {
        #region Fields

        protected int Count;
        [CLSCompliant(false)] protected Policy[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] readonly object _syncRoot = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected int Prime = 2;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] [CLSCompliant(false)] protected Metadata[] Meta;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int TO_ARRAY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int TO_ENUMERATION;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_BUILD;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_TYPE_BUILD;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY_BUILD;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_INSTANCE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_INSTANCE_BUILD;

        #endregion


        #region Constructors

        internal Defaults()
        {
            // Build Chains
            TypeChain = new StagedChain<UnityBuildStage, BuilderStrategy>();
            FactoryChain = new StagedChain<UnityBuildStage, BuilderStrategy>();
            InstanceChain = new StagedChain<UnityBuildStage, BuilderStrategy>();

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            PIPELINE_BUILD          = Allocate(typeof(PipelineFactory));
            PIPELINE_TYPE_BUILD     = Allocate(typeof(TypeCategory),     typeof(PipelineFactory));
            PIPELINE_FACTORY_BUILD  = Allocate(typeof(FactoryCategory),  typeof(PipelineFactory));
            PIPELINE_INSTANCE_BUILD = Allocate(typeof(InstanceCategory), typeof(PipelineFactory));

            // Pipelines
            PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(ResolveDelegate<PipelineContext>));
            PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(ResolveDelegate<PipelineContext>));
            PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(ResolveDelegate<PipelineContext>));

            // Enumerators
            TO_ARRAY       = Allocate(typeof(Array),       typeof(Func<Scope, Type[], Metadata[]>));
            TO_ENUMERATION = Allocate(typeof(IEnumerable), typeof(Func<Scope, Type[], Metadata[]>));
        }

        #endregion
    }
}
