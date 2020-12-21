using System;

namespace Unity
{
    public partial class UnityContainer : IServiceProvider
    {
        /// <inheritdoc />
        public object? GetService(Type serviceType)
        {
            RegistrationManager? manager;
            Contract contract = new Contract(serviceType);

            // Look for registration
            if (null != (manager = Scope.Get(in contract)))
            {
                //Registration found, check value
                var value = manager.GetValue(Scope);
                if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                // Resolve registration
                return ResolveRegistered(ref contract, manager);
            }

            // Resolve 
            return ResolveUnregistered(ref contract);
        }
    }
}
