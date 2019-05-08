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

            public override UnityContainer Container { get; }

            public override ILifetimeContainer Lifetime => Container.LifetimeContainer;

            public override IPolicyList Policies => this as IPolicyList;

            #endregion


            #region Pipelines

            public override StagedStrategyChain<Pipeline, Stage> TypePipeline { get; }

            public override StagedStrategyChain<Pipeline, Stage> FactoryPipeline { get; }

            public override StagedStrategyChain<Pipeline, Stage> InstancePipeline { get; }

            #endregion


            #region Lifetime

            public override ITypeLifetimeManager TypeLifetimeManager
            {
                get => (ITypeLifetimeManager)_typeLifetimeManager;
                set
                {
                    _typeLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _typeLifetimeManager.InUse = true;
                }
            }

            public override IFactoryLifetimeManager FactoryLifetimeManager
            {
                get => (IFactoryLifetimeManager)_factoryLifetimeManager;
                set
                {
                    _factoryLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _factoryLifetimeManager.InUse = true;
                }
            }

            public override IInstanceLifetimeManager InstanceLifetimeManager
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
