using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _sync = new();

        private int        _count;
        private Entry[]    _data;
        private Metadata[] _meta;

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
#if NETSTANDARD
            _data = new Entry[Prime.Numbers[3]];
#else
            _data = GC.AllocateUninitializedArray<Entry>(Prime.Numbers[3], false);
#endif
            _meta = new Metadata[Prime.Numbers[4]];

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
