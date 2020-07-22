using System;
using System.Diagnostics;
using System.Threading;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Public Methods

        public override void Add(LifetimeManager manager, params Type[] registerAs)
        {
            lock (_registrySync)
            {
                // Expand if required
                var required = _registryCount + registerAs.Length;
                if (required >= _registryMax) ExpandRegistry(required);

                // Iterate and register types
                foreach (var type in registerAs)
                {
                    // Skip invalid types
                    if (null == type) continue;

                    // Check for existing
                    var hash = (uint)type.GetHashCode();
                    var bucket = hash % _registryMeta.Length;
                    var position = _registryMeta[bucket].Position;
                    while (position > 0)
                    {
                        ref var candidate = ref _registryData[position];
                        if (candidate.Contract.Type == type && null == candidate.Contract.Name)
                        {
                            // Found existing
                            ReplaceManager(ref candidate, manager);
                            _version += 1;
                            break;
                        }

                        position = _registryMeta[position].Next;
                    }

                    // Add new registration
                    if (0 == position)
                    {
                        _registryData[++_registryCount] = new Registration(hash, type, manager);
                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position = _registryCount;
                        _version += 1;
                    }
                }
            }
        }


        public override void Add(in RegistrationData data)
        {
            if (null == data.Name)
            {
                Add(data.Manager, data.RegisterAs);
            }
            else
            {
                var nameInfo = GetNameInfo(data.Name);

                Add(in data, ref nameInfo);
            }
        }

        #endregion


        #region Implementation

        protected virtual void Add(in RegistrationData data, ref NameInfo identity)
        {
            lock (_registrySync)
            {
                ref var references = ref identity.References;
                var referenceCount = references[0];
                var requiredLength = data.RegisterAs.Length + referenceCount;

                // TODO: lock first 
                // Expand references if required
                if (requiredLength >= references.Length)
                    Array.Resize(ref references, (int)Math.Round(requiredLength / LoadFactor) + 1);

                // Expand registry if required
                requiredLength = _registryCount + data.RegisterAs.Length;
                if (requiredLength >= _registryMax) ExpandRegistry(requiredLength);

                // Iterate and register types
                foreach (var type in data.RegisterAs)
                {
                    // Skip invalid types
                    if (null == type) continue;

                    // Check for existing
                    var hash = type.GetHashCode(identity.Hash);
                    var bucket = hash % _registryMeta.Length;
                    var position = _registryMeta[bucket].Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _registryData[position];
                        if (hash == candidate.Hash &&
                            candidate.Contract.Type == type &&
                            ReferenceEquals(identity.Name, candidate.Contract.Name))
                        {
                            // Found existing
                            ReplaceManager(ref candidate, data.Manager);
                            _version += 1;
                            break;
                        }

                        position = _registryMeta[position].Next;
                    }

                    // Add new registration
                    if (0 == position)
                    {
                        _registryData[++_registryCount] = new Registration(hash, type, identity.Name, data.Manager);
                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position = _registryCount;
                        references[++referenceCount] = _registryCount;
                        _version += 1;
                    }
                }

                // Record new count after all done
                references[0] = referenceCount;
            }
        }


        protected ref readonly NameInfo GetNameInfo(string name)
        {
            var hash = (uint)name!.GetHashCode();
            var length = _namesCount;

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

            lock (_namesSync)
            {
                // Check again if length changed during wait for lock
                if (length != _namesCount)
                {
                    bucket = hash % _namesMeta.Length;
                    position = _namesMeta[bucket].Position;
                    while (position > 0)
                    {
                        ref var candidate = ref _namesData[position];
                        if (hash == candidate.Hash && candidate.Name == name)
                            return ref candidate;

                        position = _namesMeta[position].Next;
                    }
                }

                // Expand if required
                if (_namesCount >= _namesMax)
                {
                    var size = Prime.Numbers[++_namesPrime];
                    _namesMax = (int)(size * LoadFactor);

                    Array.Resize(ref _namesData, size);
                    _namesMeta = new Metadata[size];

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

                entry = new NameInfo(hash, name, 1);

                _namesMeta[position].Next = _namesMeta[bucket].Position;
                _namesMeta[bucket].Position = position;

                return ref entry;
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

            public NameInfo(uint hash, string? name, int size)
            {
                Hash = hash;
                Name = name;
                References = new int[size];
            }
        }

        #endregion
    }
}
