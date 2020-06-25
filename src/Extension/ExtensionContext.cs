using Unity.Policy;
using Unity.Storage;
using Unity.Pipeline;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract class ExtensionContext
    {
        #region Container

        /// <summary>
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        public abstract UnityContainer Container { get; }


        #endregion


        #region Strategies

        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; }
        
        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; }
        
        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; }

        #endregion


        #region Policy Lists

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicyList Policies { get; }

        #endregion


        #region Events

        /// <summary>
        /// This event is raised when new <see cref="Type"/> is registered.
        /// </summary>
        public abstract event RegistrationEvent TypeRegistered;

        /// <summary>
        /// This event is raised when new instance is registered.
        /// </summary>
        public abstract event RegistrationEvent InstanceRegistered;

        /// <summary>
        /// This event is raised when new instance is registered.
        /// </summary>
        public abstract event RegistrationEvent FactoryRegistered;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> 
        /// method is called and new child container is created. It allow extensions to 
        /// perform any additional initialization they may require.
        /// </summary>
        public abstract event ChildCreatedEvent ChildContainerCreated;

        #endregion
    }
}
