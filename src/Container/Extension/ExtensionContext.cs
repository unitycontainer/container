using Unity.Policy;
using Unity.Storage;
using Unity.Pipeline;
using Unity.Lifetime;
using System.Collections.Generic;
using System;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract class ExtensionContext
    {
        #region Global Settings

        #region Pipelines

        /// <summary>
        /// Pipeline chain required to process factory registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; }

        /// <summary>
        /// Pipeline chain required to process instance registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; }

        /// <summary>
        /// Pipeline chain required to process type registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; }

        #endregion


        #region Global Events

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


        /// <summary>
        /// Name of the root container
        /// </summary>
        public abstract string? Name { get; set; }

        /// <summary>
        /// The global singleton <see cref="UnityContainer"/> owner instance 
        /// </summary>
        /// <value>The <see cref="UnityContainer"/> object.</value>
        public abstract UnityContainer Container { get; }

        #endregion


        #region Root Container Settings

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicyList Policies { get; }

        /// <summary>
        /// The collection of disposable objects this container holds
        /// </summary>
        /// <value>The <see cref="ICollection{IDisposable}"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
        public abstract ICollection<IDisposable> Lifetime { get; }


        /// <summary>
        /// Default implicit lifetime of type registrations 
        /// </summary>
        public abstract ITypeLifetimeManager DefaultTypeLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of factory registrations 
        /// </summary>
        public abstract IFactoryLifetimeManager DefaultFactoryLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of instance registrations 
        /// </summary>
        public abstract IInstanceLifetimeManager DefaultInstanceLifetime { get; set; }

        #endregion
    }
}
