using System;
using Unity.Builder;
using Unity.Container;
using Unity.Container.Storage;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
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

            public override IStagedStrategyChain<UnityBuildStage> Strategies => _container._strategies;

            public override IStagedStrategyChain<UnityBuildStage> BuildPlanStrategies => _container._buildPlanStrategies;

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


            #region IPolicyList

            IBuilderPolicy IPolicyList.Get(Type policyInterface, object requestKey, out IPolicyList containingPolicyList)
            {
                Type key;
                string name = null;
                containingPolicyList = null;

                switch (requestKey)
                {
                    case null:
                        key = null;
                        break;

                    case NamedTypeBuildKey buildKey:
                        key = buildKey.Type;
                        name = buildKey.Name;
                        break;

                    case Type buildType:
                        key = buildType;
                        break;

                    default:
                        throw new ArgumentException(nameof(requestKey));
                }

                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IMap<Type, IBuilderPolicy> data;
                    if (null == (data = registry[key, name])) continue;

                    containingPolicyList = registry._context;
                    return data[policyInterface];
                }

                return null;
            }

            void IPolicyList.Set(Type policyInterface, IBuilderPolicy policy, object requestKey)
            {
                switch (requestKey)
                {
                    case null:
                        _container[null, null, policyInterface] = policy;
                        break;

                    case NamedTypeBuildKey buildKey:
                        _container[buildKey.Type, buildKey.Name, policyInterface] = policy;
                        break;

                    case Type buildType:
                        _container[buildType, null, policyInterface] = policy;
                        break;

                    default:
                        throw new ArgumentException(nameof(requestKey));
                }
            }

            void IPolicyList.Clear(Type policyInterface, object buildKey)
            {
            }

            void IPolicyList.ClearAll()
            {
                _container._registrations =
                    new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(ContainerInitialCapacity);
            }

            #endregion


            #region IPolicyRegistry

            public IMap<Type, IBuilderPolicy> this[Type type, string name] => _container[type, name];

            public IBuilderPolicy this[Type type, string name, Type policy]
            {
                get => _container[type, name, policy];
                set => _container[type, name, policy] = value;
            }

            #endregion
        }
    }
}
