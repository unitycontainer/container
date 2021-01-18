using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;
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
            TypeChain     = new StagedStrategyChain(typeof(Activator));
            FactoryChain  = new StagedStrategyChain(typeof(FactoryDelegate));
            InstanceChain = new StagedStrategyChain(typeof(CategoryInstance));
            MappingChain  = new StagedStrategyChain(typeof(Converter<,>));

            Allocate<ResolveDelegate<TContext>>(TypeChain.Type,     OnActivatePipelineChanged);
            Allocate<ResolveDelegate<TContext>>(FactoryChain.Type,  OnFactoryPipelineChanged);
            Allocate<ResolveDelegate<TContext>>(InstanceChain.Type, OnInstancePipelineChanged);
            Allocate<ResolveDelegate<TContext>>(MappingChain.Type,  OnMappingPipelineChanged);

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
