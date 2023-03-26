namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        /// <summary>
        /// Pipeline chain required to process type activations
        /// </summary>
        public abstract IActivateChain ActivateStrategies { get; }

        /// <summary>
        /// Pipeline chain holding strategies for factory registrations
        /// </summary>
        public abstract IFactoryChain FactoryStrategies { get; }

        /// <summary>
        /// Pipeline chain holding strategies for instance registrations
        /// </summary>
        public abstract IInstanceChain InstanceStrategies { get; }

        /// <summary>
        /// Pipeline chain holding strategies for mappings (From, To)
        /// </summary>
        public abstract IMappingChain MappingStrategies { get; }


        /// <summary>
        /// The factory strategies this container uses to construct build plans.
        /// </summary>
        /// <value>The <see cref="StagedStrategyChain{TStageEnum}"/> that this container uses when creating
        /// build plans.</value>
        public abstract IBuildPlanChain BuildPlanStrategies { get; }
    }
}
