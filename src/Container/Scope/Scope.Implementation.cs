using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        protected virtual void ReplaceManager(ref Registry registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
        
        }

        public override int GetHashCode()
        {
            int hash = HASH_CODE_SEED;
            var scope = this;

            do {

                hash = hash * scope._level + scope._version;
            
            } while (null != (scope = scope.Parent));

            return hash;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);
        }


        #region Hierarchy

        /// <summary>
        /// Method that creates <see cref="IUnityContainer.Registrations"/> enumerator
        /// </summary>
        public HierarchyEnumerable Hierarchy() => new HierarchyEnumerable(this);

        public struct HierarchyEnumerable : IEnumerable<ContainerScope>
        {
            private ContainerScope _containerScope;

            public HierarchyEnumerable(ContainerScope containerScope) => _containerScope = containerScope;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ContainerScope> GetEnumerator() => new HierarchyEnumerator(_containerScope);

            public struct HierarchyEnumerator : IEnumerator<ContainerScope>
            {
                private ContainerScope  _current;
                private ContainerScope? _next;

                public HierarchyEnumerator(ContainerScope containerScope)
                {
                    _current = containerScope;
                    _next    = containerScope;
                }

                object IEnumerator.Current => Current;

                public ContainerScope Current => _current!;

                public bool MoveNext()
                {
                    if (null == _next) return false;
                    
                    _current = _next;
                    _next = _current.Parent;

                    return true;
                }
                
                public void Dispose() { }

                public void Reset() => throw new NotSupportedException();
            }
        }

        #endregion
    }
}
