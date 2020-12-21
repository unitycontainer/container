using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async ValueTask RegisterAsync(params RegistrationDescriptor[] descriptors)
        {
            throw new NotImplementedException();
            //ReadOnlyMemory<RegistrationDescriptor> memory = new ReadOnlyMemory<RegistrationDescriptor>(descriptors);

            //// Register with the scope
            //await Task.Factory.StartNew(_scope.AddAsync, memory,
            //    System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

            //// Report registration
            //_registering?.Invoke(this, memory.Span);
        }

        /// <inheritdoc />
        public async ValueTask RegisterAsync(ReadOnlyMemory<RegistrationDescriptor> memory, TaskScheduler? scheduler = null)
        {
            throw new NotImplementedException();
            //// Register with the scope
            //await Task.Factory.StartNew(_scope.AddAsync, memory,
            //    System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach,
            //    scheduler ?? TaskScheduler.Default);

            //// Report registration
            //_registering?.Invoke(this, memory.Span);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

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
                if (null != (manager = container!.Scope.Get(in contract)))
                {
                    object? value;

                    // Registration found, check for value
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(Scope)))
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


        #region Child Container

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer(string? name, int capacity)
            => CreateChildContainer(name, capacity);

        #endregion
    }
}
