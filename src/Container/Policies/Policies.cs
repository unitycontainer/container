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
            // Build Chains & subscribe to change notifications
            TypeChain     = new StagedStrategyChain(typeof(Activator));
            FactoryChain  = new StagedStrategyChain(typeof(FactoryDelegate));
            InstanceChain = new StagedStrategyChain(typeof(CategoryInstance));

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];


            // Resolve Unregistered Type
            Allocate<ResolveDelegate<TContext>>(OnResolveUnregisteredChanged);

            // Resolve Registered Type
            Allocate<ResolveDelegate<TContext>>(typeof(ContainerRegistration), 
                                                OnResolveRegisteredChanged);
            // Resolve Array
            Allocate<ResolveDelegate<TContext>>(typeof(Array),
                                                OnResolveArrayChanged);


            // Type Pipeline
            Allocate<ResolveDelegate<TContext>>(typeof(Activator),
                                                OnActivatePipelineChanged);
            // Factory Pipeline
            Allocate<ResolveDelegate<TContext>>(typeof(FactoryDelegate),
                                                OnFactoryPipelineChanged);
            // Instance Pipeline
            Allocate<ResolveDelegate<TContext>>(typeof(CategoryInstance),
                                                OnInstancePipelineChanged);


            // Type Pipeline Factory
            Allocate<PipelineFactory<TContext>>(OnPipelineFactoryChanged);
        }

        #endregion
    }
}
