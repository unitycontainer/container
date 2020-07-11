using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        protected void ReplaceManager(ref Registry registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
        
        }

        #region Container Hierarchy

        protected HierarchyEnumerable Hierarchy() => new HierarchyEnumerable(this);

        protected readonly struct HierarchyEnumerable : IEnumerable<ContainerScope>
        {
            private readonly ContainerScope _scope;

            public HierarchyEnumerable(ContainerScope scope) 
                => _scope = scope;

            readonly IEnumerator<ContainerScope> IEnumerable<ContainerScope>.GetEnumerator() 
                => GetEnumerator();

            readonly IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();

            public readonly Enumerator<ContainerScope> GetEnumerator()
            {
                var scope = _scope;
                return new Enumerator<ContainerScope>((out ContainerScope current) =>
                {
                    current = scope;

                    if (null == scope) return false;

                    scope = scope.Parent;

                    return true;
                });
            }
        }

        #endregion
    }
}
