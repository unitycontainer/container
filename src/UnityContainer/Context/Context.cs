using System;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <inheritdoc />
        public partial class ContainerContext : IDisposable
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

            public LifetimeManager TypeLifetimeManager
            {
                get => _typeLifetimeManager;
                set
                {
                    if (!(value is ITypeLifetimeManager)) 
                        throw new ArgumentException($"{value} must implement {nameof(ITypeLifetimeManager)} interface");
                   
                    _typeLifetimeManager = value;
                }
            }

            public LifetimeManager FactoryLifetimeManager
            {
                get => _factoryLifetimeManager;
                set
                {
                    if (!(value is IFactoryLifetimeManager)) 
                        throw new ArgumentException($"{value} must implement {nameof(IFactoryLifetimeManager)} interface");

                    _factoryLifetimeManager = value;
                }
            }

            public LifetimeManager InstanceLifetimeManager
            {
                get => _instanceLifetimeManager;
                set
                {
                    if (!(value is IInstanceLifetimeManager)) 
                        throw new ArgumentException($"{value} must implement {nameof(IInstanceLifetimeManager)} interface");

                    _instanceLifetimeManager = value;
                }
            }

            #endregion


            #region IDisposable

            public void Dispose()
            {
                TypePipeline.Invalidated -= OnTypePipelineChanged;
                FactoryPipeline.Invalidated -= OnFactoryPipelineChanged;
                InstancePipeline.Invalidated -= OnInstancePipelineChanged;
            }

            #endregion
        }
    }
}
