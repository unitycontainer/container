using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Container;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        protected override bool MoveNext(ref int index, ref ContainerRegistration registration)
        {
            index = index + 1;

            if (RunningIndex < index) return false;

            ref var entry = ref _contractData[index];

            // TODO: Requires optimization
            registration = new ContainerRegistration(in entry._contract, (LifetimeManager)entry._manager);

            return true;
        }

        public override int GetHashCode()
        {
            int hash = HASH_CODE_SEED;
            var scope = this;

            do {

                hash = hash * scope._level + scope._version;
            
            } while (null != (scope = (ContainerScope?)scope.Next));

            return hash;
        }

        #region Contracts

        protected virtual int Add(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref _contractMeta[hash % _contractMeta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (candidate._contract.Type == contract.Type && ReferenceEquals(
                    candidate._contract.Name, contract.Name))
                {
                    // Found existing
                    candidate = new ContainerRegistration(in contract, manager);
                    _version += 1;
                    return 0;
                }

                position = _contractMeta[position].Next;
            }

            // Add new registration
            RunningIndex++;
            _contractData[RunningIndex] = new ContainerRegistration(in contract, manager);
            _contractMeta[RunningIndex].Next = bucket.Position;
            bucket.Position = (int)RunningIndex;
            _version += 1;

            return (int)RunningIndex;
        }


        protected override void Expand(long required)
        {
            var size = Prime.GetNext((int)(required * ReLoadFactor));

            _contractMeta = new Metadata[size];
            _contractMeta.Setup(LoadFactor);

            base.Expand(_contractMeta.Capacity());

            for (var current = START_INDEX; current <= RunningIndex; current++)
            {
                var bucket = (uint)_contractData[current]._contract.HashCode % size;
                _contractMeta[current].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = current;
            }
        }

        #endregion


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
                    _next = (ContainerScope?)_current.Next;

                    return true;
                }
                
                public void Dispose() { }

                public void Reset() => throw new NotSupportedException();
            }
        }

        #endregion
    }
}
