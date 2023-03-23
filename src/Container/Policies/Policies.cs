using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;
using static Unity.IUnityContainer;

namespace Unity.Container
{
    public partial class Policies<TContext> where TContext : IBuilderContext
    {
        #region Fields

        protected int Count;
        
        [CLSCompliant(false)] 
        protected Policy[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CLSCompliant(false)] 
        protected Metadata[] Meta;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        protected readonly object SyncRoot = new object();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        protected int Prime = 3;

        #endregion


        #region Constructors

        internal Policies()
        {
            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Build Chains
            TypeChain     = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>(typeof(Activator));
            FactoryChain  = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>(typeof(FactoryDelegate));
            InstanceChain = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>(typeof(CategoryInstance));
            MappingChain  = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>(typeof(Converter<,>));

            Allocate<ResolveDelegate<TContext>>(typeof(Activator),     OnActivatePipelineChanged);
            Allocate<ResolveDelegate<TContext>>(typeof(FactoryDelegate),  OnFactoryPipelineChanged);
            Allocate<ResolveDelegate<TContext>>(typeof(CategoryInstance), OnInstancePipelineChanged);
            Allocate<ResolveDelegate<TContext>>(typeof(Converter<,>),  OnMappingPipelineChanged);

            // Resolve Unregistered Type
            Allocate<ResolveDelegate<TContext>>(OnResolveUnregisteredChanged);

            // Resolve Registered Type
            Allocate<ResolveDelegate<TContext>>(typeof(ContainerRegistration), 
                                                OnResolveRegisteredChanged);
            // Resolve Array
            Allocate<ResolveDelegate<TContext>>(typeof(Array),
                                                OnResolveArrayChanged);

            // Pipeline Factories
            Allocate<PipelineFactory<TContext>>(OnPipelineFactoryChanged);
        }

        #endregion
    }
}
