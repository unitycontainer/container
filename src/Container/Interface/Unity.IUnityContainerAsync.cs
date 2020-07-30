using System;
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

        async ValueTask IUnityContainerAsync.RegisterAsync(ReadOnlyMemory<RegistrationDescriptor> memory)
        {
            await Task.Factory.StartNew( (a) => Register(memory.Span), memory, System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            await Task.Run(() => Register(memory.Span));
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
        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer(string? name) => CreateChildContainer(name);
        #endregion
    }
}
