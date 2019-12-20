using System;
using Unity.Events;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="IExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public interface IExtensionContext
    {
        #region Container

        /// <summary>
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        UnityContainer Container { get; }

        /// <summary>
        /// The <see cref="ILifetimeContainer"/> that this container uses.
        /// </summary>
        /// <value>The <see cref="ILifetimeContainer"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
        ILifetimeContainer Lifetime { get; }

        #endregion


        #region Pipeline Build Chains

        IStagedStrategyChain<Pipeline, Stage> TypePipeline { get; }
        
        IStagedStrategyChain<Pipeline, Stage> FactoryPipeline { get; }
        
        IStagedStrategyChain<Pipeline, Stage> InstancePipeline { get; }

        #endregion


        #region Default Lifetime

        ITypeLifetimeManager     TypeLifetimeManager { get; set; }
        
        IFactoryLifetimeManager  FactoryLifetimeManager { get; set; }
        
        IInstanceLifetimeManager InstanceLifetimeManager { get; set; }

        #endregion


        #region Policy List

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        IPolicyList Policies { get; }

        #endregion


        // TODO: Events

        #region Events

        /// <summary>
        /// This event is raised when the 
        /// <see cref="IUnityContainer.RegisterType(Type,Type,string,LifetimeManager, InjectionMember[])"/> 
        /// method, or one of its overloads, is called.
        /// </summary>
        event EventHandler<RegisterEventArgs> Registering;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
        /// or one of its overloads, is called.
        /// </summary>
        event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

        /// <summary>
        /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> method is called, providing 
        /// the newly created child container to extensions to act on as they see fit.
        /// </summary>
        event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        #endregion
    }
}
