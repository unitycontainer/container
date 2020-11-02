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
            if (null != (manager = _scope.Get(in contract)))
            {
                //Registration found, check value
                var value = manager.TryGetValue(_scope);
                if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                // Resolve registration
                return ResolveSilent(ref contract, manager);
            }

            // Resolve 
            return ResolveSilent(ref contract);
        }
    }
}
