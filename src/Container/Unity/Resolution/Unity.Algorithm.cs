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
        #region Public API

        /// <inheritdoc />
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            var contract = new Contract(type, name);
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;

            do
            {
                // Look for registration
                var manager = container._scope.Get(in contract);
                if (null != manager)
                {
                    //Registration found, check value
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope.Disposables);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return container.ResolveRegistration(ref contract, manager, overrides);
                }

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
                  : type.IsArray ? ResolveArray(ref contract, overrides)
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
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope.Disposables)))
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

        #endregion


        #region Dependency

        internal object? Resolve(ref Contract contract, ref PipelineContext parent)
        {
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;

            do
            {
                // Look for registration
                var manager = container._scope.Get(in contract);
                if (null != manager)
                {
                    //Registration found, check value
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope.Disposables);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return container.ResolveRegistration(ref contract, manager, ref parent);
                }

                // Skip to parent if non generic
                if (!(isGeneric ??= contract.Type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = container._scope.Get(in contract, in generic)))
                {
                    // Build from generic factory
                    return container.GenericRegistration(ref contract, manager, ref parent);
                }
            }
            while (null != (container = container.Parent));

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref contract, ref generic, ref parent)
                : contract.Type.IsArray
                    ? ResolveArray(ref contract, ref parent)
                    : ResolveUnregistered(ref contract, ref parent);
        }

        #endregion


        #region Async

        /// <summary>
        /// Builds and resolves registered contract
        /// </summary>
        /// <param name="state"><see cref="ResolveContractAsyncState"/> objects holding 
        /// resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveContractAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;

            return Task.FromResult<object?>(context.Manager);
        }

        /// <summary>
        /// Builds and resolves unregistered <see cref="Type"/>
        /// </summary>
        /// <param name="state"><see cref="RequestInfoAsync"/> objects holding resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;

            return Task.FromResult<object?>(context.Contract.Type);
        }

        #endregion
    }
}
