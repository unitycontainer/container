using System;
using System.Reflection;
using Unity.Events;
using Unity.Injection;
using Unity.Lifetime;
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
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        public abstract UnityContainer Container { get; }

        /// <summary>
        /// The <see cref="ILifetimeContainer"/> that this container uses.
        /// </summary>
        /// <value>The <see cref="ILifetimeContainer"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
        public abstract ILifetimeContainer Lifetime { get; }

        #endregion


        #region Pipeline Build Chains

        public abstract StagedStrategyChain<PipelineBuilder, PipelineStage> TypePipeline { get; set; }
        public abstract StagedStrategyChain<PipelineBuilder, PipelineStage> FactoryPipeline { get; set; }
        public abstract StagedStrategyChain<PipelineBuilder, PipelineStage> InstancePipeline { get; set; }

        #endregion


        #region Default Lifetime

        public abstract ITypeLifetimeManager     TypeLifetimeManager { get; set; }
        public abstract IFactoryLifetimeManager  FactoryLifetimeManager { get; set; }
        public abstract IInstanceLifetimeManager InstanceLifetimeManager { get; set; }

        #endregion


        #region Policy List

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicyList Policies { get; }

        #endregion


        // TODO: Events

        #region Events

        /// <summary>
        /// This event is raised when the 
        /// <see cref="IUnityContainer.RegisterType(Type,Type,string,LifetimeManager, InjectionMember[])"/> 
        /// method, or one of its overloads, is called.
        /// </summary>
        public abstract event EventHandler<RegisterEventArgs> Registering;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
        /// or one of its overloads, is called.
        /// </summary>
        public abstract event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> method is called, providing 
        /// the newly created child container to extensions to act on as they see fit.
        /// </summary>
        public abstract event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        #endregion
    }
}
