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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int TO_ARRAY_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int TO_ENUMERATION;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY_CONTEXT;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_INSTANCE;

        #endregion


        #region Constructors

        internal Defaults()
        {
            // Build Chains
            TypeChain     = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(TypeCategory));
            FactoryChain  = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(FactoryCategory));
            InstanceChain = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(InstanceCategory));

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            PIPELINE_FACTORY_TYPE    = Allocate(typeof(PipelineFactory<PipelineContext>));
            PIPELINE_FACTORY_CONTEXT = Allocate(typeof(ContextualFactory<PipelineContext>));

            // Pipelines
            PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(ResolveDelegate<PipelineContext>));
            PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(ResolveDelegate<PipelineContext>));
            PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(ResolveDelegate<PipelineContext>));

            // Enumerators
            TO_ARRAY       = Allocate(typeof(Array),       typeof(Func<Scope, Type[], Metadata[]>));
            TO_ARRAY_TYPE  = Allocate(typeof(Array),       typeof(Func<UnityContainer, Type, Type>));
            TO_ENUMERATION = Allocate(typeof(IEnumerable), typeof(Func<Scope, Type[], Metadata[]>));
        }

        #endregion
    }
}
