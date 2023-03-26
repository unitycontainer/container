namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        #region Container

        /// <summary>
        /// The instance of extended container
        /// </summary>
        /// <value>The instance of <see cref="UnityContainer"/></value>
        public abstract UnityContainer Container { get; }

        /// <summary>
        /// The collection of disposable objects this container is responsible for
        /// </summary>
        /// <value>The <see cref="ICollection{IDisposable}"/> of <see cref="IDisposable"/> objects</value>
        public abstract ICollection<IDisposable> Lifetime { get; }

        #endregion


        #region Strategies

        /// <summary>
        /// Pipeline chain required to process type activations
        /// </summary>
        public abstract IActivateChain ActivateStrategies { get; }

        /// <summary>
        /// Pipeline chain holding strategies for instance registrations
        /// </summary>
        public abstract IInstanceChain InstanceStrategies { get; }

        /// <summary>
        /// Pipeline chain holding strategies for factory registrations
        /// </summary>
        public abstract IFactoryChain FactoryStrategies { get; }

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

        #endregion


        #region Policy Lists

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicies Policies { get; }

        #endregion


        #region Events

        /// <summary>
        /// This event is raised on new registration
        /// </summary>
        public abstract event RegistrationEvent Registering;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> 
        /// method is called and new child container is created. It allow extensions to 
        /// perform any additional initialization they may require.
        /// </summary>
        public abstract event ChildCreatedEvent ChildContainerCreated;

        #endregion
    }
}
