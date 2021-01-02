using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Fields

        protected int Count;
        [CLSCompliant(false)] protected Policy[] Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] [CLSCompliant(false)] protected Metadata[] Meta;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected readonly object SyncRoot = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected int Prime = 2;

        #endregion


        #region Constructors

        internal Policies()
        {
            // Build Chains & subscribe to change notifications
            TypeChain     = new StagedStrategyChain(typeof(Activator));
            FactoryChain  = new StagedStrategyChain(typeof(IUnityContainer.FactoryDelegate));
            InstanceChain = new StagedStrategyChain(typeof(CategoryInstance));

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            Allocate<PipelineFactory<PipelineContext>>(OnPipelineFactoryChanged);
            Allocate<PipelineFactory<PipelineContext>>(typeof(Type),
                                                       OnFromTypeFactoryChanged);

            // Algorithms
            Allocate<ResolveDelegate<PipelineContext>>(OnResolveUnregisteredChanged);
            Allocate<ResolveDelegate<PipelineContext>>(typeof(ContainerRegistration), 
                                                       OnResolveRegisteredChanged);
            Allocate<ResolveDelegate<PipelineContext>>(typeof(Array),
                                                       OnResolveArrayChanged);

            // Pipelines
            Allocate<ResolveDelegate<PipelineContext>>(typeof(Activator),
                                                       OnActivatePipelineChanged);
            Allocate<ResolveDelegate<PipelineContext>>(typeof(IUnityContainer.FactoryDelegate),
                                                       OnFactoryPipelineChanged);
            Allocate<ResolveDelegate<PipelineContext>>(typeof(CategoryInstance),
                                                       OnInstancePipelineChanged);
        }

        #endregion
    }
}
