using System;
using System.Runtime.CompilerServices;
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
            var contract = new Contract(type, name);
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;

            do
            {
                RegistrationManager? manager;

                // Optimistic lookup
                if (null != (manager = container._scope.Get(in contract)))
                {
                    object? value;

                    // Registration found, check for value
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope.Disposables)))
                        return value;

                    // Build is required
                    return container.ResolveContract(in contract, manager, overrides);
                }

                // Skip to parent if non generic
                if (!(isGeneric ??= type.IsGenericType())) continue;

                // Fill the Generic Type Definition
                if (null == generic.Type) generic = contract.With(type.GetGenericTypeDefinition());

                // Attempt to get from user factory, if such factory exists
                if (null != (manager = container._scope.Get(in contract, in generic)))
                {
                    // Build from user factory
                    return container.ResolveContractGeneric(in contract, manager, overrides);
                }
            }
            while (null != (container = container.Parent));

            // Check if resolver already exist
            var resolver = _policies[contract.Type];
            if (null != resolver)
            {
                var context = new Pipeline.ResolveContext(this, in contract, overrides);
                return resolver(ref context);
            }

            // No registration found, resolve unregistered
            return (isGeneric ?? false)
                ? ResolveUnregisteredGeneric(in contract, in generic, overrides)
                : ResolveUnregistered(in contract, overrides);
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
