﻿using System;
using System.Collections.Generic;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Storage;

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
        /// Pipeline chain required to process factory registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, BuilderStage> FactoryPipeline { get; }

        /// <summary>
        /// Pipeline chain required to process instance registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, BuilderStage> InstancePipeline { get; }

        /// <summary>
        /// Pipeline chain required to process type registrations
        /// </summary>
        public abstract IStagedStrategyChain<PipelineProcessor, BuilderStage> TypePipeline { get; }

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