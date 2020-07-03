using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope : IEnumerable<ContainerRegistration>
        {
            #region IEnumerable

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerRegistration> GetEnumerator()
            {
                var set = new QuickSet<Type>();

                // Scan containers for explicit registrations
                for (ContainerScope? scope = this; null != scope; scope = scope._parent)
                {
                    for (var i = 0; i < scope._registryCount; i++)
                    {
                        var entry = scope._registry[i];

                        if (!(entry.Manager is LifetimeManager manager) ||
                            !set.Add(entry.Type, entry.HashCode)) continue;
                        
                        yield return new ContainerRegistration(entry.Type, entry.Name, manager);
                    }
                }
            }

            #endregion
        }
    }
}
