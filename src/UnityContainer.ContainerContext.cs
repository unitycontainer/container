using System;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
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


            #region IPolicyList

            public virtual void ClearAll()
            {
            }

            public virtual object Get(Type type, string name, Type policyInterface) 
                => _container.GetPolicy(type, name, policyInterface);

            public virtual void Set(Type type, string name, Type policyInterface, object policy)
                => _container.SetPolicy(type, name, policyInterface, policy);

            public virtual void Clear(Type type, string name, Type policyInterface)
            {
            }

            #endregion
        }
    }
}
