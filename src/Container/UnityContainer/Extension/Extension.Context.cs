using System.Diagnostics;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity
{
    // Extension Management
    public sealed partial class UnityContainer 
    {
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
            public override ILifetimeContainer Lifetime => Container.LifetimeContainer;

            #endregion


            #region Pipelines

            /// <inheritdoc />
            public override IActivationChain ActivateStrategies
                => Container.Policies.ActivationChain;

            /// <inheritdoc />
            public override IFactoryChain FactoryStrategies 
                => Container.Policies.FactoryChain;

            /// <inheritdoc />
            public override IInstanceChain InstanceStrategies 
                => Container.Policies.InstanceChain;

            /// <inheritdoc />
            public override IMappingChain MappingStrategies 
                => Container.Policies.MappingChain;

            public override IBuildPlanChain BuildPlanStrategies
                => Container.Policies.BuildPlanChain;

            #endregion


            #region Declarations

            /// <inheritdoc />
            public override GetConstructorsSelector? GetConstructors
            {
                get => Policies.Get<GetConstructorsSelector>();
                set => Policies.Set(value ?? throw new ArgumentNullException(nameof(GetConstructors)));
            }

            /// <inheritdoc />
            public override GetFieldsSelector? GetFields
            {
                get => Policies.Get<GetFieldsSelector>();
                set => Policies.Set(value ?? throw new ArgumentNullException(nameof(GetFields)));
            }

            /// <inheritdoc />
            public override GetPropertiesSelector? GetProperties
            {
                get => Policies.Get<GetPropertiesSelector>();
                set => Policies.Set(value ?? throw new ArgumentNullException(nameof(GetProperties)));
            }

            /// <inheritdoc />
            public override GetMethodsSelector? GetMethods
            {
                get => Policies.Get<GetMethodsSelector>();
                set => Policies.Set(value ?? throw new ArgumentNullException(nameof(GetMethods)));
            }

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
