using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Properties

        /// <inheritdoc />
        IUnityContainer? IUnityContainer.Parent => Parent;

        #endregion


        #region Registration

        /// <inheritdoc />
        public IUnityContainer Register(params RegistrationDescriptor[] descriptors)
        {
            ReadOnlySpan<RegistrationDescriptor> span = descriptors;

            // Register with the scope
            _scope.Add(in span);

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            // Register with the scope
            _scope.Add(in span);

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        #endregion


        #region Resolution

        /// <inheritdoc />
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            RegistrationManager? manager;
            Contract contract = new Contract(type, name);
            Contract definition = default;
            bool? isGeneric = null;
            var container = this;

            do
            {
                // Optimistic lookup
                if (null != (manager = container._scope.Get(in contract)))
                {
                    object? value;

                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope.Disposables)))
                        return value;

                    // Requires build

                    break;
                }
                
                // Skip to parent if not generic
                if (!(isGeneric ??= type.IsGenericType())) continue;

                // Fill the Generic Type Definition
                if (null == definition.Type)
                    definition = contract.With(type.GetGenericTypeDefinition());

                // Get from factory
                if (null != (manager = container._scope.Get(in contract, in definition))) 
                {
                    // Requires build from factory

                    break;
                }
            }
            while (null != (container = container.Parent));

            // Requires build from scratch

            return manager;
        }


        private RegistrationManager? Get(in Contract contract)
        {
            return new ContainerLifetimeManager(this);
        }






        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Child Container

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IUnityContainer IUnityContainer.CreateChildContainer(string? name, int capacity)
            => CreateChildContainer(name, capacity);

        #endregion
    }
}
