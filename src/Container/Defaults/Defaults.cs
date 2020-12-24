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
            TypeChain     = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(CategoryType));
            FactoryChain  = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(CategoryFactory));
            InstanceChain = new StagedChain<UnityBuildStage, BuilderStrategy>(typeof(CategoryInstance));

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            PIPELINE_FACTORY        = Allocate<PipelineFactory<PipelineContext>>();
            RESOLVER_FACTORY        = Allocate<ResolverFactory<PipelineContext>>();

            // Pipelines
            BUILD_PIPELINE_TYPE     = Allocate<CategoryType,     ResolveDelegate<PipelineContext>>();
            BUILD_PIPELINE_FACTORY  = Allocate<CategoryFactory,  ResolveDelegate<PipelineContext>>();
            BUILD_PIPELINE_INSTANCE = Allocate<CategoryInstance, ResolveDelegate<PipelineContext>>();

            // Collections
            GET_TARGET_TYPE = Allocate<Array, UnitySelector<Type, Type>>();
        }

        #endregion
    }
}
