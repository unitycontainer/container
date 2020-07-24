using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (_registryCount < index) return false;

            ref var entry = ref _registryData[index];

            // TODO: Requires optimization
            registration = new ContainerRegistration(in entry.Contract, (LifetimeManager)entry.Manager);

            return true;
        }

        protected virtual void ReplaceManager(ref Registration registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
            _version += 1;
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

        #region Registrations

        protected virtual void Add(in RegistrationDescriptor data, ref NameInfo identity)
        {
            ref var references = ref identity.References;
            var referenceCount = references[0];
            var requiredLength = data.RegisterAs.Length + referenceCount;

            // Expand references if required
            if (requiredLength >= references.Length)
                Array.Resize(ref references, (int)(requiredLength * ReLoadFactor) + 1);

            // Expand registry if required
            requiredLength = _registryCount + data.RegisterAs.Length;
            if (requiredLength >= _registryMeta.MaxIndex()) ExpandRegistry(requiredLength);

            // Iterate and register types
            foreach (var type in data.RegisterAs)
            {
                // Skip invalid types
                if (null == type) continue;

                // Check for existing
                var hash = type.GetHashCode(identity.Hash);
                ref var bucket = ref _registryMeta[hash % _registryMeta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _registryData[position];
                    if (hash == candidate.Hash &&
                        candidate.Contract.Type == type &&
                        ReferenceEquals(identity.Name, candidate.Contract.Name))
                    {
                        // Found existing
                        ReplaceManager(ref candidate, data.Manager);
                        break;
                    }

                    position = _registryMeta[position].Next;
                }

                // Add new registration
                if (0 == position)
                {
                    _registryData[++_registryCount] = new Registration(hash, type, identity.Name, data.Manager);
                    _registryMeta[_registryCount].Next = bucket.Position;
                    bucket.Position = _registryCount;
                    references[++referenceCount] = _registryCount;
                    _version += 1;
                }
            }

            // Record new count after all done
            references[0] = referenceCount;
        }

        protected virtual void ExpandRegistry(int required)
        {
            var size = Prime.GetNext((int)(required * ReLoadFactor));

            _registryMeta = new Metadata[size];
            _registryMeta.Setup(LoadFactor);

            Array.Resize(ref _registryData, _registryMeta.GetCapacity());

            for (var current = START_INDEX; current <= _registryCount; current++)
            {
                var bucket = _registryData[current].Hash % size;
                _registryMeta[current].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = current;
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

                Array.Resize(ref _namesData, _namesMeta.GetCapacity());

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
        }

        #endregion
    }
}
