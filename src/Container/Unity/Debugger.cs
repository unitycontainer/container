using System.Collections.Generic;
using System.Diagnostics;

namespace Unity
{
    [DebuggerDisplay("UnityContainer[{Registered}]", Name = "{ Name,nq }")]
    [DebuggerTypeProxy(typeof(UnityContainerProxy))]
    public partial class UnityContainer
    {
        private int Registered => _scope.Count;

        private class UnityContainerProxy
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private UnityContainer _container;

            public UnityContainerProxy(UnityContainer container) => _container = container;

            public string? Name => _container.Name;

            public IEnumerable<ContainerRegistration> Registrations => _container.Registrations;

            public UnityContainer? Parent => _container._parent;
        }
    }
}
