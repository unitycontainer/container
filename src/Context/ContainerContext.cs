using System;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;
using Unity.Strategy;

namespace Unity
{
    public partial class UnityContainer
    {

        /// <summary>
        /// Abstraction layer between container and extensions
        /// </summary>
        /// <remarks>
        /// Implemented as a nested class to gain access to  
        /// container that would otherwise be inaccessible.
        /// </remarks>
        private class ContainerContext : ExtensionContext, 
                                         IContainerContext, 
                                         IPolicyList 
        {
            #region Fields

            private readonly UnityContainer _container;

            #endregion


            #region Constructors

            public ContainerContext(UnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
            }

            public T Policy<T>(Type type, string name) where T : IBuilderPolicy
            {
                return (T)_container[type, name, typeof(T)];
            }

            public void Policy<T>(Type type, string name, T value) where T : IBuilderPolicy
            {
                _container[type, name, typeof(T)] = value;
            }


            #endregion


            #region ExtensionContext

            public override IUnityContainer Container => _container;

            public override IStagedStrategyChain<IBuilderStrategy, UnityBuildStage> Strategies => _container._strategies;

            public override IStagedStrategyChain<IBuilderStrategy, BuilderStage> BuildPlanStrategies => _container._buildPlanStrategies;

            public override IPolicyList Policies => _container._context;

            public override ILifetimeContainer Lifetime => _container._lifetimeContainer;

            public override event EventHandler<RegisterEventArgs> Registering
            {
                add => _container.Registering += value;
                remove => _container.Registering -= value;
            }

            public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add => _container.RegisteringInstance += value;
                remove => _container.RegisteringInstance -= value;
            }

            public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add => _container.ChildContainerCreated += value;
                remove => _container.ChildContainerCreated -= value;
            }

            #endregion


            #region IContainerContext

            /// <summary>
            /// Retrieves registration for requested named type
            /// </summary>
            /// <param name="type">Registration type</param>
            /// <param name="name">Registration name</param>
            /// <param name="create">Instruncts container if it should create registration if not found</param>
            /// <returns>Registration for requested named type or null if named type is not registered and 
            /// <see cref="create"/> is false</returns>
            public INamedType Registration(Type type, string name, bool create = false)
            {
                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IPolicyStore data;
                    if (null == (data = registry[type, name])) continue;

                    return (INamedType)data;
                }

                if (!create) return null;

                return null;
            }

            #endregion


            #region IPolicyList

            void IPolicyList.ClearAll()
            {
                _container._registrations =
                    new HashRegistry<Type, IRegistry<string, IPolicyStore>>(ContainerInitialCapacity);
            }

            public IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IPolicyStore data;
                    if (null == (data = registry[type, name])) continue;

                    list = registry._context;
                    return data.Get(policyInterface);
                }

                list = null;
                return null;
            }

            public void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IPolicyStore data;
                    if (null == (data = registry[type, name])) continue;

                    data.Set(policyInterface, policy);
                    return;
                }

                _container[type, name, policyInterface] = policy;
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
