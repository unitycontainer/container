using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        #region Strategies

        /// <summary>
        /// Pipeline chain required to process type registrations
        /// </summary>
        [Obsolete($"This chain was split into {nameof(ActivateStrategies)}, {nameof(InstanceStrategies)}, and {nameof(MappingStrategies)} chains", true)]
        public virtual IStagedStrategyChain<BuilderStrategy, UnityBuildStage>? Strategies { get; }

        #endregion
    }
}
