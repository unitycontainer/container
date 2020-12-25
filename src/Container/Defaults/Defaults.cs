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
            Allocate<PipelineFactory<PipelineContext>>((_, _, policy)
                => PipelineFactory = (PipelineFactory<PipelineContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy))));

            Allocate<ResolverFactory<PipelineContext>>((_, _, policy)
                => ResolverFactory = (ResolverFactory<PipelineContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy))));

            
            // Pipelines
            Allocate<CategoryType, ResolveDelegate<PipelineContext>>((_, _, policy)
                => TypePipeline = (ResolveDelegate<PipelineContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy))));

            Allocate<CategoryFactory, ResolveDelegate<PipelineContext>>((_, _, policy)
                => FactoryPipeline = (ResolveDelegate<PipelineContext>)(policy ??
                   throw new ArgumentNullException(nameof(policy))));

            Allocate<CategoryInstance, ResolveDelegate<PipelineContext>>((_, _, policy)
                => InstancePipeline = (ResolveDelegate<PipelineContext>)(policy ??
                    throw new ArgumentNullException(nameof(policy))));

            
            // Collections
            Allocate<Array, UnitySelector<Type, Type>>((_, _, policy)
                => ArrayTargetType = (UnitySelector<Type, Type>)(policy ??
                    throw new ArgumentNullException(nameof(policy))));
        }

        #endregion
    }
}
