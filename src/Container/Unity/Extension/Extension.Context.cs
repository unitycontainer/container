using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Builder;
using Unity.Extension;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    // Extension Management
    public partial class UnityContainer : IEnumerable<Type>
    {
        #region Fields

        private PrivateExtensionContext? _context;
        private event RegistrationEvent? _registering;
        private event ChildCreatedEvent? _childContainerCreated;
        private List<IUnityContainerExtensionConfigurator>? _extensions;

        #endregion


        #region Events

        protected event RegistrationEvent Registering
        {
            add 
            { 
                // TODO: Registration propagation?
                //if (null != Parent && _registering is null)
                //    Parent.Registering += OnParentRegistering;

                _registering += value; 
            }

            remove 
            { 
                _registering -= value;

                //if (_registering is null && null != Parent)
                //    Parent.Registering -= OnParentRegistering;
            }
        }

        // TODO: Find better place 
        private void OnParentRegistering(object container, in ReadOnlySpan<RegistrationDescriptor> registrations) 
            => _registering?.Invoke(container, in registrations);

        protected event ChildCreatedEvent ChildContainerCreated
        {
            add => _childContainerCreated += value;
            remove => _childContainerCreated -= value;
        }

        #endregion


        #region IEnumerable

        /// <summary>
        /// This method returns <see cref="IEnumerator{Type}"/> with types of available 
        /// managed extensions
        /// </summary>
        /// <remarks>
        /// Extensions, after executing method <see cref="UnityContainerExtension.Initialize"/>,
        /// either discarded or added to the container's storage based on implementation of 
        /// <see cref="IUnityContainerExtensionConfigurator"/> interface:
        /// <para>If extension implements <see cref="IUnityContainerExtensionConfigurator"/> interface,
        /// it is stored in the container and kept alive until the container goes out of scope.</para>
        /// <para>If extension does not implement the interface, its reference is released 
        /// immediately after initialization</para>
        /// </remarks>
        /// <returns><see cref="Type"/> enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Type>)this).GetEnumerator();

        /// <summary>
        /// This method returns <see cref="IEnumerator{Type}"/> with types of available 
        /// managed extensions
        /// </summary>
        /// <remarks>
        /// Extensions, after executing method <see cref="UnityContainerExtension.Initialize"/>,
        /// either discarded or added to the container's storage based on implementation of 
        /// <see cref="IUnityContainerExtensionConfigurator"/> interface:
        /// <para>If extension implements <see cref="IUnityContainerExtensionConfigurator"/> interface,
        /// it is stored in the container and kept alive until the container goes out of scope.</para>
        /// <para>If extension does not implement the interface, its reference is released 
        /// immediately after initialization</para>
        /// </remarks>
        /// <returns><see cref="Type"/> enumerator</returns>
        IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
        {
            if (_extensions is null) yield break;

            foreach (var extension in _extensions)
                yield return extension.GetType();
        }

        #endregion


        #region Extension Context implementation

        /// <summary>
        /// Implementation of the ExtensionContext that is used extension management.
        /// </summary>
        /// <remarks>
        /// This is a nested class so that it can access state in the container that 
        /// would otherwise be inaccessible.
        /// </remarks>
        [DebuggerTypeProxy(typeof(ExtensionContext))]
        private class PrivateExtensionContext : ExtensionContext
        {
            #region Constructors

            public PrivateExtensionContext(UnityContainer container) => Container = container;

            #endregion


            #region Container

            /// <inheritdoc />
            public override UnityContainer Container { get; }

            /// <inheritdoc />
            public override IPolicies Policies => Container.Policies;

            /// <inheritdoc />
            public override ICollection<IDisposable> Lifetime => Container.Scope;

            #endregion


            #region Pipelines

            /// <inheritdoc />
            public override IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage> FactoryStrategies 
                => Container.Policies.FactoryChain;

            /// <inheritdoc />
            public override IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage> InstanceStrategies 
                => Container.Policies.InstanceChain;

            /// <inheritdoc />
            public override IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage> Strategies 
                => Container.Policies.StrategiesChain;

            /// <inheritdoc />
            public override IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage> MappingStrategies 
                => Container.Policies.MappingChain;

            public override IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage> BuildPlanStrategies
                => Container.Policies.BuildPlanChain;

            #endregion


            #region Events

            /// <inheritdoc />
            public override event RegistrationEvent Registering
            {
                add    => Container.Registering += value;
                remove => Container.Registering -= value;
            }

            /// <inheritdoc />
            public override event ChildCreatedEvent ChildContainerCreated
            {
                add    => Container.ChildContainerCreated += value;
                remove => Container.ChildContainerCreated -= value;
            }

            #endregion
        }

        #endregion
    }
}
