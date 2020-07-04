using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {
            public IEnumerable<ContainerRegistration> Registrations => new RegistrationsSet(this);


            #region Registrations

            [DebuggerDisplay("Registrations")]
            private class RegistrationsSet : IEnumerable<ContainerRegistration>
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private ContainerScope _scope;

                public RegistrationsSet(ContainerScope scope) => _scope = scope;

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                public IEnumerator<ContainerRegistration> GetEnumerator()
                {
                    var set = new QuickSet<Type>();

                    // Scan containers for explicit registrations
                    for (ContainerScope? scope = _scope; null != scope; scope = scope._parent)
                    {
                        for (var i = START_INDEX; i <= scope._registryCount; i++)
                        {
                            var entry = scope._registryData[i];

                            if (RegistrationType.Internal == entry.Manager.RegistrationType ||
                                !(entry.Manager is LifetimeManager manager)                 ||
                                !set.Add(entry.Type, entry.HashCode)) continue;

                            yield return new ContainerRegistration(entry.Type, entry.Name, manager);
                        }
                    }
                }
            }

            #endregion
        }
    }
}
