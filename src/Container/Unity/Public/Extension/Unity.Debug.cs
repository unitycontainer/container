using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

            public UnityContainerProxy(UnityContainer container)
            {
                _container = container;
                Registrations = new RegistrationsDebugView(container);
            }

            #endregion


            #region Visible Members

            public string? Name => _container.Name;

            public RegistrationsDebugView Registrations { get; }

            public UnityContainer? Parent => _container.Parent;

            #endregion


            #region Nested 

            [DebuggerDisplay("Contracts = {Items.Length}, Version = {Version}")]
            public class RegistrationsDebugView
            {
                public RegistrationsDebugView(UnityContainer container)
                {
                    Items = container.Registrations
                                     .ToArray();
                
                    Version = container.Scope.Version;
                }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public readonly int Version;

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public ContainerRegistration[] Items { get; }
            }

            #endregion
        }
    }
}
