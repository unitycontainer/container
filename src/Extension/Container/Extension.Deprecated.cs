using System;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        [Obsolete("Use 'TypePipelineChain', 'InstancePipelineChain', or 'FactoryPipelineChain' instead", false)]
        public IStagedStrategyChain Strategies => TypePipelineChain;
    }

}
