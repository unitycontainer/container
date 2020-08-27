using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainerAsync
    {
        #region Properties

        /// <inheritdoc />
        IUnityContainerAsync? IUnityContainerAsync.Parent => Parent;

        #endregion


        #region Registration

        /// <inheritdoc />
        public async ValueTask RegisterAsync(params RegistrationDescriptor[] descriptors)
        {
            ReadOnlyMemory<RegistrationDescriptor> memory = new ReadOnlyMemory<RegistrationDescriptor>(descriptors);

            // Register with the scope
            await Task.Factory.StartNew(_scope.AddAsync, memory,
                System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

            // Report registration
            _registering?.Invoke(this, memory.Span);
        }

        /// <inheritdoc />
        public async ValueTask RegisterAsync(ReadOnlyMemory<RegistrationDescriptor> memory, TaskScheduler? scheduler = null)
        {
            // Register with the scope
            await Task.Factory.StartNew(_scope.AddAsync, memory,
                System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                scheduler ?? TaskScheduler.Default);

            // Report registration
            _registering?.Invoke(this, memory.Span);
        }

        #endregion


        #region Resolution

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
                    return new ValueTask<object?>(Task.Factory.StartNew(container.ResolveContractAsync, new ResolveContractAsyncState(in contract, manager, overrides),
                        System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
                }
            }
            while (null != (container = container.Parent));

            // No registration found, do everything else asynchronously
            return new ValueTask<object?>(
                Task.Factory.StartNew(ResolveAsync, new ResolveAsyncState(in contract, overrides),
                    System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
        }

        #endregion


        #region Child Container

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer(string? name, int capacity)
            => CreateChildContainer(name, capacity);

        #endregion
    }
}
