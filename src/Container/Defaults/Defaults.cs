using System;
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int GET_TARGET_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_INSTANCE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY_CONTEXT;

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

            // Collections
            GET_TARGET_TYPE = Allocate(typeof(Array), typeof(Func<UnityContainer, Type, Type>));
        }

        #endregion
    }
}
