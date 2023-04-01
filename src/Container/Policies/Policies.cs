using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies
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

        private IBuildPlanChain?  _buildPlanChain;
        private IActivationChain? _activationChain;
        private IInstanceChain?   _instanceChain;
        private IFactoryChain?    _factoryChain;
        private IMappingChain?    _mappingChain;

        #endregion


        #region Constructors

        internal Policies()
        {
            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Resolve Unregistered Type
            Allocate<ResolverPipeline>(OnResolveUnregisteredChanged);

            // Resolve Registered Type
            Allocate<ResolverPipeline>(typeof(ContainerRegistration), OnResolveRegisteredChanged);

            // Resolve Array
            Allocate<ResolverPipeline>(typeof(Array), OnResolveArrayChanged);

            Allocate<ChainToPipelineConverter>(OnChainToPipelineConverterChanged);
            Allocate<ChainToFactoryConverter>(OnChainToFactoryConverterChanged);
        }

        #endregion
    }
}
