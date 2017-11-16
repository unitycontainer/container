using System;
using Unity.Builder;
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
        /// Implementation of the ExtensionContext that is actually used
        /// by the UnityContainer implementation.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the
        /// container that would otherwise be inaccessible.
        /// </remarks>
        private class ContainerContext : ExtensionContext
        {
            private readonly UnityContainer _container;

            public ContainerContext(UnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
            }

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

            /// <summary>
            /// This event is raised when the <see cref="RegisterInstance(Type,string,object,LifetimeManager)"/> method,
            /// or one of its overloads, is called.
            /// </summary>
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
        }

        private class ContainerPolicyList : IPolicyList
        {
            private readonly UnityContainer _container;

            public ContainerPolicyList(UnityContainer container)
            {
                _container = container;
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

                    case IBuildKey buildKey:
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
                    IIndexerOf<Type, IBuilderPolicy> data;
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

                    case IBuildKey buildKey:
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
                throw new NotImplementedException();
            }
        }
    }
}
