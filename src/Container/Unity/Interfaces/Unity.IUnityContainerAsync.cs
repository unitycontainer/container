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
            throw new NotImplementedException();
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
