using System;
using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainerAsync
    {
        #region Properties

        /// <inheritdoc />
        IUnityContainerAsync? IUnityContainerAsync.Parent => _parent;

        public ValueTask<object?> ResolveAsync(Type type, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Resolution

        #endregion


        /// <inheritdoc />
        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer(string? name) => CreateChildContainer(name);
    }
}
