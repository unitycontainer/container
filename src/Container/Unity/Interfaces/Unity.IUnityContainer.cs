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
            //var context = new ContainerContext(type, name, overrides);
            //var registration = Get(in contract, out UnityContainer? container);
            //var registration = Get(ref context);
            
            RegistrationManager? manager;
            //bool? isGeneric = null;

            Contract contract = new Contract(type, name);
            var container = this;


            do
            {
                // Optimistic get
                if (null != (manager = container._scope.Get(in contract))) break;
                //if (!(isGeneric ??= type.IsGenericType())) continue;
                //Contract definition = contract.With(type.GetGenericTypeDefinition());

                if (null != (manager = Get(in contract))) break;
            }
            while (null != (container = container.Parent));

            //manager = _scope.Get(in contract) ?? Get(in contract);

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
