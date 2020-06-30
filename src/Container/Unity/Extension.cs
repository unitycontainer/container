using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    // Extension Management
    public partial class UnityContainer : IEnumerable
    {
        #region Fields

        private List<UnityContainerExtension>? _extensions;

        #endregion


        #region IEnumerable

        /// <summary>
        /// This method returns <see cref="IEnumerator<Type>"/> with types of installed extensions
        /// </summary>
        /// <returns><see cref="Type"/> enumerator of extensions</returns>
        public IEnumerator GetEnumerator()
        {
            if (null == _extensions) yield break;

            foreach (var extension in _extensions)
            {
                yield return extension.GetType();
            }
        }

        #endregion


        #region Root Extension Context class

        /// <summary>
        /// Implementation of the ExtensionContext that is used extension management.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the container that 
        /// would otherwise be inaccessible.
        /// </remarks>
        [DebuggerTypeProxy(typeof(ExtensionContext))]
        private class RootExtensionContext : ExtensionContext
        {
            #region Fields

            private event RegistrationEvent? _typeRegistered;
            private event RegistrationEvent? _instanceRegistered;
            private event RegistrationEvent? _factoryRegistered;
            private event ChildCreatedEvent? _childContainerCreated;

            #endregion


            #region Constructors

            public RootExtensionContext(UnityContainer container)
            {
                Container = container;

                FactoryPipeline  = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
                InstancePipeline = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
                TypePipeline     = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
            }

            #endregion


            #region Root Container

            /// <inheritdoc />
            public override string? Name 
            { 
                get => Container._name;
                set => Container._name = value; 
            }

            /// <inheritdoc />
            public override UnityContainer Container { get; }

            /// <inheritdoc />
            public override IPolicyList Policies => Container._scope;

            /// <inheritdoc />
            public override ICollection<IDisposable> Lifetime => Container._scope.Lifetime;

            #endregion


            #region Pipelines


            /// <inheritdoc />
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; }

            /// <inheritdoc />
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; }

            /// <inheritdoc />
            public override IStagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; }

            #endregion


            #region Lifetimes

            /// <inheritdoc />
            public override ITypeLifetimeManager DefaultTypeLifetime
            {
                get => (ITypeLifetimeManager)Container._typeLifetime;
                set => Container._typeLifetime = (LifetimeManager)value;
            }

            /// <inheritdoc />
            public override IFactoryLifetimeManager DefaultFactoryLifetime
            {
                get => (IFactoryLifetimeManager)Container._factoryLifetime;
                set => Container._factoryLifetime = (LifetimeManager)value;
            }

            /// <inheritdoc />
            public override IInstanceLifetimeManager DefaultInstanceLifetime
            {
                get => (IInstanceLifetimeManager)Container._instanceLifetime;
                set => Container._instanceLifetime = (LifetimeManager)value;
            }

            #endregion


            #region Events

            /// <inheritdoc />
            public override event RegistrationEvent TypeRegistered
            {
                add    => _typeRegistered += value;
                remove => _typeRegistered -= value;
            }

            /// <inheritdoc />
            public override event RegistrationEvent InstanceRegistered
            {
                add    => _instanceRegistered += value;
                remove => _instanceRegistered -= value;
            }

            /// <inheritdoc />
            public override event RegistrationEvent FactoryRegistered
            {
                add    => _factoryRegistered += value;
                remove => _factoryRegistered -= value;
            }

            /// <inheritdoc />
            public override event ChildCreatedEvent ChildContainerCreated
            {
                add    => _childContainerCreated += value;
                remove => _childContainerCreated -= value;
            }

            #endregion


            #region Implementation

            public bool RaiseTypeRegistered     => null != _typeRegistered;
            public bool RaiseInstanceRegistered => null != _instanceRegistered;
            public bool RaiseFactoryRegistered  => null != _factoryRegistered;
            public bool RaiseChildCreated       => null != _childContainerCreated;

            public void OnChildContainerCreated(ContainerContext context)       => _childContainerCreated!(context);
            public void OnTypeRegistered(ref RegistrationManager registration)     => _typeRegistered!(ref registration);
            public void OnInstanceRegistered(ref RegistrationManager registration) => _instanceRegistered!(ref registration);
            public void OnFactoryRegistered(ref RegistrationManager registration)  => _factoryRegistered!(ref registration);

            #endregion
        }

        #endregion
    }
}
