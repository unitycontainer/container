using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Defaults
    {
        #region Fields

        protected int Count;
        [CLSCompliant(false)] protected Policy[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] [CLSCompliant(false)] protected Metadata[] Meta;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] readonly object _syncRoot = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected int Prime = 2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int PIPELINE_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int RESOLVER_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int GET_TARGET_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int BUILD_PIPELINE_TYPE;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int BUILD_PIPELINE_FACTORY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int BUILD_PIPELINE_INSTANCE;

        #endregion


        #region Constructors

        internal Defaults()
        {
            // Build Chains & subscribe to change notifications
            TypeChain = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(TypeCategory));
            FactoryChain = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(FactoryCategory));
            InstanceChain = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(InstanceCategory));

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            PIPELINE_FACTORY         = Allocate(typeof(PipelineFactory<PipelineContext>));
            RESOLVER_FACTORY         = Allocate(typeof(ResolverFactory<PipelineContext>));

            // Pipelines
            BUILD_PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(ResolveDelegate<PipelineContext>));
            BUILD_PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(ResolveDelegate<PipelineContext>));
            BUILD_PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(ResolveDelegate<PipelineContext>));

            // Collections
            GET_TARGET_TYPE = Allocate(typeof(Array), typeof(Func<UnityContainer, Type, Type>));
        }

        #endregion


        #region Properties

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);

        #endregion
    }
}
