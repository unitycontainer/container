using System;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <inheritdoc />
        public partial class ContainerContext
        {
            #region Extension Context

            public UnityContainer Container { get; }

            public ILifetimeContainer Lifetime => Container.LifetimeContainer;

            public IPolicyList Policies => this as IPolicyList;

            #endregion


            #region Pipelines

            public  IStagedStrategyChain<Pipeline, Stage> TypePipeline { get; }

            public  IStagedStrategyChain<Pipeline, Stage> FactoryPipeline { get; }

            public  IStagedStrategyChain<Pipeline, Stage> InstancePipeline { get; }

            #endregion


            #region Lifetime

            public ITypeLifetimeManager TypeLifetimeManager
            {
                get => (ITypeLifetimeManager)_typeLifetimeManager;
                set
                {
                    _typeLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _typeLifetimeManager.InUse = true;
                }
            }

            public IFactoryLifetimeManager FactoryLifetimeManager
            {
                get => (IFactoryLifetimeManager)_factoryLifetimeManager;
                set
                {
                    _factoryLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _factoryLifetimeManager.InUse = true;
                }
            }

            public IInstanceLifetimeManager InstanceLifetimeManager
            {
                get => (IInstanceLifetimeManager)_instanceLifetimeManager;
                set
                {
                    _instanceLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _instanceLifetimeManager.InUse = true;
                }
            }

            #endregion
        }
    }
}
