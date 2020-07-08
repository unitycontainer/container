using System;
using System.Diagnostics;
using System.Threading;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        #region Register

        protected void RegisterAnonymous(ref RegistrationData data)
        {
            lock (_registrySync)
            {
                // Expand if required
                var required = _registryCount + data.RegisterAs.Length;
                if (required >= _registryMax) ExpandRegistry(required);

                // Iterate and register types
                foreach (var type in data.RegisterAs)
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
                        if (candidate.Type == type && 0 == candidate.Identity)
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
                        ref var entry = ref _registryData[++_registryCount];
                        entry.Hash = hash;
                        entry.Type = type;
                        entry.Identity = 0;
                        entry.Manager  = data.Manager;

                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position = _registryCount;
                    }
                }
            }
        }

        protected void RegisterContracts(ref RegistrationData data)
        {
            var nameHash = (uint)data.Name!.GetHashCode();
            var nameIndex = IndexOf(nameHash, data.Name, data.RegisterAs.Length);

            lock (_registrySync)
            {
                ref var references = ref _contractData[nameIndex].References;
                var referenceCount = references[0];
                var requiredLength = data.RegisterAs.Length + referenceCount;

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
                    var hash = type.GetHashCode(nameHash);
                    var bucket = hash % _registryMeta.Length;
                    var position = _registryMeta[bucket].Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _registryData[position];
                        if (candidate.Type == type && nameIndex == candidate.Identity)
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
                        ref var entry = ref _registryData[++_registryCount];
                        entry.Hash = hash;
                        entry.Type = type;
                        entry.Manager = data.Manager;
                        entry.Identity = nameIndex;

                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position = _registryCount;
                        references[++referenceCount] = _registryCount;
                    }
                }

                // Record new count after all done
                references[0] = referenceCount;
            }

            return;
        }

        #endregion


        #region Register Asynchronously

        public virtual void RegisterAnonymous(ref RegistrationData data, CancellationToken token)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual void RegisterContracts(ref RegistrationData data, CancellationToken token)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        #endregion


        #region Contracts

        protected int IndexOf(uint hash, string? name, int required)
        {
            var length = _contractCount;

            // Check if already registered
            var bucket = hash % _contractMeta.Length;
            var position = _contractMeta[bucket].Position;
            while (position > 0)
            {
                if (_contractData[position].Name == name) return position;
                position = _contractMeta[position].Next;
            }

            lock (_contractSync)
            {
                // Check again if length changed during wait for lock
                if (length != _contractCount)
                {
                    bucket = hash % _contractMeta.Length;
                    position = _contractMeta[bucket].Position;
                    while (position > 0)
                    {
                        if (_contractData[position].Name == name) return position;
                        position = _contractMeta[position].Next;
                    }
                }

                // Expand if required
                if (_contractCount >= _contractMax)
                {
                    var size = Prime.Numbers[++_contractPrime];
                    _contractMax = (int)(size * LoadFactor);

                    Array.Resize(ref _contractData, size);
                    _contractMeta = new Metadata[size];

                    // Rebuild buckets
                    for (var index = START_INDEX; index <= _contractCount; index++)
                    {
                        bucket = _contractData[index].Hash % size;
                        _contractMeta[index].Next = _contractMeta[bucket].Position;
                        _contractMeta[bucket].Position = index;
                    }

                    bucket = hash % _contractMeta.Length;
                }

                ref var entry = ref _contractData[++_contractCount];
                entry.Name = name;
                entry.Hash = hash;
                entry.References = new int[required + 1];

                _contractMeta[_contractCount].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = _contractCount;

                return _contractCount;
            }
        }

        #endregion


        #region Expanding

        protected void ExpandRegistry(int required)
        {
            var index = Prime.IndexOf((int)(required / LoadFactor));
            int size = Prime.Numbers[index];

            _registryMax = (int)(size * LoadFactor);

            Array.Resize(ref _registryData, size);
            _registryMeta = new Metadata[size];

            for (var i = START_INDEX; i <= _registryCount; i++)
            {
                var bucket = _registryData[i].Hash % size;
                _registryMeta[i].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = i;
            }
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("Identity = { Identity }, Manager = {Manager}", Name = "{ (Type?.Name ?? string.Empty),nq }")]
        public struct Registry
        {
            public uint Hash;
            public Type Type;
            public int Identity;
            public RegistrationManager Manager;
        }

        [DebuggerDisplay("{ Name }")]
        public struct Contract
        {
            public uint Hash;
            public string? Name;
            public int[] References;
        }

        #endregion
    }
}
