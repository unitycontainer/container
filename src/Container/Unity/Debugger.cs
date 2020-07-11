using System.Collections.Generic;
using System.Diagnostics;

namespace Unity
{
    [DebuggerDisplay("UnityContainer[{Name ?? GetHashCode().ToString()}]")]
    [DebuggerTypeProxy(typeof(UnityContainerProxy))]
    public partial class UnityContainer
    {
        /// <summary>
        /// Proxy class that represents container during debugging
        /// </summary>
        private class UnityContainerProxy
        {
            #region Scaffolding

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private UnityContainer _container;

            public UnityContainerProxy(UnityContainer container) => _container = container;

            #endregion


            #region Visible Members

            public string? Name => _container.Name;

            //public IEnumerable<ContainerRegistration> Registrations => _container.Registrations;

            public UnityContainer? Parent => _container.Parent;

            #endregion
        }
    }
}
