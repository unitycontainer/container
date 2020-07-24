using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
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

            if (_contractCount < index) return false;

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
            
            } while (null != (scope = (ContainerScope?)scope._next));

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
            _contractCount++;
            _contractData[_contractCount] = new ContainerRegistration(in contract, manager);
            _contractMeta[_contractCount].Next = bucket.Position;
            bucket.Position = _contractCount;
            _version += 1;

            return _contractCount;
        }


        protected virtual void ExpandRegistry(int required)
        {
            var size = Prime.GetNext((int)(required * ReLoadFactor));

            _contractMeta = new Metadata[size];
            _contractMeta.Setup(LoadFactor);

            Array.Resize(ref _contractData, _contractMeta.Capacity());

            for (var current = START_INDEX; current <= _contractCount; current++)
            {
                var bucket = (uint)_contractData[current]._contract.HashCode % size;
                _contractMeta[current].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = current;
            }
        }

        #endregion


        #region Names

        protected ref readonly NameInfo GetNameInfo(string name)
        {
            var hash = (uint)name!.GetHashCode();

            // Check if already registered
            var bucket = hash % _namesMeta.Length;
            var position = _namesMeta[bucket].Position;
            while (position > 0)
            {
                ref var candidate = ref _namesData[position];
                if (hash == candidate.Hash && candidate.Name == name)
                    return ref candidate;

                position = _namesMeta[position].Next;
            }

            // Expand if required
            if (_namesCount >= _namesMeta.MaxIndex())
            {
                var size = Prime.Numbers[++_namesPrime];

                _namesMeta = new Metadata[size];
                _namesMeta.Setup(LoadFactor);

                Array.Resize(ref _namesData, _namesMeta.Capacity());

                // Rebuild buckets
                for (var current = START_INDEX; current <= _namesCount; current++)
                {
                    bucket = _namesData[current].Hash % size;
                    _namesMeta[current].Next = _namesMeta[bucket].Position;
                    _namesMeta[bucket].Position = current;
                }

                bucket = hash % _namesMeta.Length;
            }

            position = Interlocked.Increment(ref _namesCount);
            ref var entry = ref _namesData[position];

            entry = new NameInfo(hash, name);

            _namesMeta[position].Next = _namesMeta[bucket].Position;
            _namesMeta[bucket].Position = position;

            return ref entry;
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
                    _next = (ContainerScope?)_current._next;

                    return true;
                }
                
                public void Dispose() { }

                public void Reset() => throw new NotSupportedException();
            }
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("{ Name }")]
        public struct NameInfo
        {
            public readonly uint Hash;
            public readonly string? Name;
            public int[] References;

            public NameInfo(uint hash, string? name)
            {
                Hash = hash;
                Name = name;
                References = new int[5];
            }

            public void Resize(int required)
            {
                var requiredLength = required + References[0] + 1;

                // Expand references if required
                if (requiredLength >= References.Length)
                    Array.Resize(ref References, (int)(requiredLength * 1.45f));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Register(int position) 
                => References[++References[0]] = position;
        }

        #endregion
    }
}
