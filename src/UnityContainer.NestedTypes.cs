using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container;
using Unity.Container.Storage;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
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
        private class ContainerContext : ExtensionContext, IContext
        {
            private readonly UnityContainer _container;

            public ContainerContext(UnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
            }

            #region ExtensionContext

            public override IUnityContainer Container => _container;

            public override IStagedStrategyChain<UnityBuildStage> Strategies => _container._strategies;

            public override IStagedStrategyChain<UnityBuildStage> BuildPlanStrategies => _container._buildPlanStrategies;

            public override IPolicyList Policies => _container._policies;

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


            #region IContext

            IContext IContext.Parent => _container._parent?._context;

            IEnumerable<IContainerRegistration> IContext.this[Type type] => throw new NotImplementedException();

            IContainerRegistration IContext.this[Type type, string name] => (IContainerRegistration)_container[type, name];

            IBuilderPolicy IContext.this[Type type, string name, Type policy] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            #endregion
        }

        private class ContainerPolicyList : IPolicyList
        {
            private readonly UnityContainer _container;

            public ContainerPolicyList(UnityContainer container)
            {
                _container = container;
                if (null != _container.Parent) _container[null, null] = ((UnityContainer)_container.Parent)[null, null];
            }

            public IBuilderPolicy Get(Type policyInterface, object requestKey, out IPolicyList containingPolicyList)
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

                    containingPolicyList = registry._policies;
                    return data[policyInterface];
                }

                return null;
            }

            public void Set(Type policyInterface, IBuilderPolicy policy, object requestKey = null)
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

            public void Clear(Type policyInterface, object buildKey)
            {
            }

            public void ClearAll()
            {
                _container._registrations =
                    new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(ContainerInitialCapacity);
            }
        }
    }
}
