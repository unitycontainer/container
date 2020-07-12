using System;
using System.Diagnostics;
using System.Threading;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        #region Register

        protected virtual void RegisterAnonymous(ref RegistrationData data)
        {
            lock (_lifetimes)
            {
                // Expand if required
                var required = _registrations + data.RegisterAs.Length;
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
                        if (candidate.Contract.Type == type && 0 == candidate.Identity)
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
                        _registryData[++_registrations]    = new Registry(hash, type, data.Manager);
                        _registryMeta[_registrations].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position     = _registrations;
                        _version += 1;
                    }
                }
            }
        }

        protected virtual void RegisterContracts(ref RegistrationData data)
        {
            var nameHash = (uint)data.Name!.GetHashCode();
            var nameIndex = IndexOf(nameHash, data.Name, data.RegisterAs.Length);

            lock (_lifetimes)
            {
                ref var references = ref _contractData[nameIndex].References;
                var referenceCount = references[0];
                var requiredLength = data.RegisterAs.Length + referenceCount;

                // Expand references if required
                if (requiredLength >= references.Length)
                    Array.Resize(ref references, (int)Math.Round(requiredLength / LoadFactor) + 1);

                // Expand registry if required
                requiredLength = _registrations + data.RegisterAs.Length;
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
                        if (candidate.Contract.Type == type && nameIndex == candidate.Identity)
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
                        _registryData[++_registrations]    = new Registry(hash, type, data.Name, nameIndex, data.Manager);
                        _registryMeta[_registrations].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position     = _registrations;
                        references[++referenceCount]       = _registrations;
                        _version += 1;
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

        protected virtual int IndexOf(uint hash, string? name, int required)
        {
            var length = _contracts;

            // Check if already registered
            var bucket = hash % _contractMeta.Length;
            var position = _contractMeta[bucket].Position;
            while (position > 0)
            {
                if (_contractData[position].Name == name) return position;
                position = _contractMeta[position].Next;
            }

            lock (_manager)
            {
                // Check again if length changed during wait for lock
                if (length != _contracts)
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
                if (_contracts >= _contractMax)
                {
                    var size = Prime.Numbers[++_contractPrime];
                    _contractMax = (int)(size * LoadFactor);

                    Array.Resize(ref _contractData, size);
                    _contractMeta = new Metadata[size];

                    // Rebuild buckets
                    for (var current = START_INDEX; current <= _contracts; current++)
                    {
                        bucket = _contractData[current].Hash % size;
                        _contractMeta[current].Next = _contractMeta[bucket].Position;
                        _contractMeta[bucket].Position = current;
                    }

                    bucket = hash % _contractMeta.Length;
                }

                _contractData[++_contracts] = new Identity(hash, name, required + 1);
                _contractMeta[_contracts].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = _contracts;

                return _contracts;
            }
        }

        #endregion


        #region Expanding

        protected virtual void ExpandRegistry(int required)
        {
            var index = Prime.IndexOf((int)(required / LoadFactor));
            int size = Prime.Numbers[index];

            _registryMax = (int)(size * LoadFactor);

            Array.Resize(ref _registryData, size);
            _registryMeta = new Metadata[size];

            for (var current = START_INDEX; current <= _registrations; current++)
            {
                var bucket = _registryData[current].Hash % size;
                _registryMeta[current].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = current;
            }
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("Identity = { Identity }, Manager = {Manager}", Name = "{ (Contract.Type?.Name ?? string.Empty),nq }")]
        public struct Registry
        {
            public readonly uint Hash;
            public readonly int  Identity;
            public readonly Contract   Contract;
            public RegistrationManager Manager;

            public Registry(uint hash, Type type, RegistrationManager manager)
            {
                Hash = hash;
                Contract = new Contract(type);
                Manager  = manager;
                Identity = 0;
            }

            public Registry(uint hash, Type type, string name, int identity, RegistrationManager manager)
            {
                Hash = hash;
                Contract = new Contract(type, name);
                Manager = manager;
                Identity = identity;
            }
        }

        [DebuggerDisplay("{ Name }")]
        public struct Identity
        {
            public readonly uint Hash;
            public readonly string? Name;
            public int[] References;

            public Identity(uint hash, string? name, int size)
            {
                Hash = hash;
                Name = name;
                References = new int[size];
            }
        }

        #endregion
    }
}
