using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <inheritdoc />
//#if NET5_0
//        [SkipLocalsInit]
//#endif
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            var contract = new Contract(type, name);
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;
            RegistrationCategory cutoff = RegistrationCategory.Uninitialized;

            do
            {
                // Look for registration
                var manager = container._scope.Get(in contract, cutoff);
                if (null != manager)
                {
                    //Registration found, check value
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return container.ResolveRegistration(ref contract, manager, overrides);
                }

                cutoff = RegistrationCategory.Cache;

                // Skip to parent if non generic
                if (!(isGeneric ??= type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = contract.With(type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = container._scope.Get(in contract, in generic)))
                {
                    // Build from generic factory
                    return container.GenericRegistration(ref contract, manager, overrides);
                }
            }
            while (null != (container = container.Parent));

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref contract, ref generic, overrides)
                  : type.IsArray   ? ResolveUnregisteredArray(ref contract, overrides)
                                   : ResolveUnregistered(ref contract, overrides);
        }
        

        /// <inheritdoc />
        public ValueTask<object?> ResolveAsync(Type type, string? name, params ResolverOverride[] overrides)
        {
            var contract = new Contract(type, name);
            var container = this;

            do
            {
                RegistrationManager? manager;

                // Optimistic lookup
                if (null != (manager = container!._scope.Get(in contract)))
                {
                    object? value;

                    // Registration found, check for value
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope)))
                        return new ValueTask<object?>(value);

                    // No value, do everything else asynchronously
                    return new ValueTask<object?>(Task.Factory.StartNew(container.ResolveContractAsync, new RequestInfoAsync(in contract, manager, overrides),
                        System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
                }
            }
            while (null != (container = container.Parent));

            // No registration found, do everything else asynchronously
            return new ValueTask<object?>(
                Task.Factory.StartNew(ResolveAsync, new RequestInfoAsync(in contract, overrides),
                    System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
        }
    }
}
