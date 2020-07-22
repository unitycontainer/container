using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        protected override bool MoveNext(ref int index, ref Container.Scope.Registration registration)
        {
            index = index + 1;

            if (_registryCount < index) return false;

            registration = _registryData[index];

            return true;
        }


        protected virtual void ReplaceManager(ref Registration registry, RegistrationManager manager)
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
            
            } while (null != (scope = (ContainerScope?)scope._next));

            return hash;
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
                    _next = (ContainerScope?)_current._next;

                    return true;
                }
                
                public void Dispose() { }

                public void Reset() => throw new NotSupportedException();
            }
        }

        #endregion
    }
}
