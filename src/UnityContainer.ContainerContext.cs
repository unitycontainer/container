using System;
using Unity.Builder;
using Unity.Builder.Strategy;
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

            private readonly object syncRoot = new object();
            private readonly UnityContainer _container;

            #endregion


            #region Constructors

            public ContainerContext(UnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
                Policies = this;
            }

            #endregion


            #region ExtensionContext

            public override IUnityContainer Container => _container;

            public override IStagedStrategyChain<BuilderStrategy, UnityBuildStage> Strategies
            {
                get
                {
                    if (null != _container._parent && 
                        _container._parent._strategies == _container._strategies)
                    {
                        lock (syncRoot)
                        {
                            if (_container._parent._strategies == _container._strategies)
                            {
                                _container._strategies.Invalidated -= _container.OnStrategiesChanged;
                                _container._strategies = 
                                    new StagedStrategyChain<BuilderStrategy, UnityBuildStage>(_container._parent._strategies);
                                _container._strategies.Invalidated += _container.OnStrategiesChanged;
                                _container._lifetimeContainer.Add(_container._strategies);

                            }
                        }
                    }

                    return _container._strategies;
                }
            }

            public override IStagedStrategyChain<BuilderStrategy, BuilderStage> BuildPlanStrategies
            {
                get
                {
                    if (null != _container._parent && 
                        _container._parent._buildPlanStrategies == _container._buildPlanStrategies)
                    {
                        lock (syncRoot)
                        {
                            if (_container._parent._buildPlanStrategies == _container._buildPlanStrategies)
                            {
                                _container._buildPlanStrategies = 
                                    new StagedStrategyChain<BuilderStrategy, BuilderStage>(_container._parent._buildPlanStrategies);
                                _container._lifetimeContainer.Add(_container._buildPlanStrategies);
                            }
                        }
                    }

                    return _container._buildPlanStrategies;
                }
            }

            public override IPolicyList Policies { get; }

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

            public IContainerContext RegistrationContext(InternalRegistration registration)
            {
                return new RegistrationContext(_container, registration);
            }

            #endregion


            #region IPolicyList

            public virtual void ClearAll()
            {
                _container._registrations =
                    new HashRegistry<Type, IRegistry<string, IPolicySet>>(ContainerInitialCapacity);
            }

            public virtual IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IPolicySet data;
                    if (null == (data = registry[type, name])) continue;

                    list = registry._context;
                    return data.Get(policyInterface);
                }

                list = null;
                return null;
            }

            public virtual void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                for (var registry = _container; null != registry; registry = registry._parent)
                {
                    IPolicySet data;
                    if (null == (data = registry[type, name])) continue;

                    data.Set(policyInterface, policy);
                    return;
                }

                _container[type, name, policyInterface] = policy;
            }

            public virtual void Clear(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private class RegistrationContext : ContainerContext
        {
            private readonly InternalRegistration _registration;

            internal RegistrationContext(UnityContainer container, InternalRegistration registration)
                : base(container)
            {
                _registration = registration;
            }


            #region IPolicyList

            public override IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                if (_registration.Type != type || _registration.Name != name)
                    return base.Get(type, name, policyInterface, out list);

                list = this;
                return _registration.Get(policyInterface);
            }


            public override void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                if (_registration.Type != type || _registration.Name != name)
                    base.Set(type, name, policyInterface, policy);

                _registration.Set(policyInterface, policy);
            }

            public override void Clear(Type type, string name, Type policyInterface)
            {
                if (_registration.Type != type || _registration.Name != name)
                    base.Clear(type, name, policyInterface);

                _registration.Clear(policyInterface);
            }

            #endregion
        }
    }
}
