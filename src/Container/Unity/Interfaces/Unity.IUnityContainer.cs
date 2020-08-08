using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Pipeline;
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
            Contract generics = default;
            Contract contract = new Contract(type, name);
            bool? isGeneric = null;
            var container = this;

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

                    // Requires build
                    var context = new ContainerContext(container, in contract, overrides);

                    if (null != manager.ResolveDelegate) 
                        return ((ResolveDelegate<ResolveContext>)manager.ResolveDelegate)(ref context.ResolveContext);

                    return manager.Category switch
                    {
                        RegistrationCategory.Type => _policies.TypeResolver(ref context.ResolveContext),
                        RegistrationCategory.Factory => _policies.FactoryResolver(ref context.ResolveContext),
                        RegistrationCategory.Instance => _policies.InstanceResolver(ref context.ResolveContext),
                        _ => throw new InvalidOperationException($"Unknown Registration: { manager }")
                    };
                }
                
                // Skip to parent if not generic
                if (!(isGeneric ??= type.IsGenericType())) continue;

                // Fill the Generic Type Definition
                if (null == generics.Type)
                    generics = contract.With(type.GetGenericTypeDefinition());

                // Get from factory
                if (null != (manager = container._scope.Get(in contract, in generics)))
                {
                    // Build from factory
                    var context = new ContainerContext(container, in contract, overrides);

                    return _policies.TypeResolver(ref context.ResolveContext);
                }
            }
            while (null != (container = container.Parent));


            ResolveDelegate<ResolveContext>? resolver;

            // Check if type factory exists
            if ((isGeneric ?? false) && (null != (resolver = _policies[generics.Type])))
            {
            }

            // Check if array
            if (type.IsArray)
            {
                resolver = _policies[typeof(Array)];
            }

            // Requires build from scratch

            return null;
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
