using System;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Register

        protected virtual void RegisterAnonymous(in RegistrationData data)
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
                        if (candidate.Contract.Type == type && null == candidate.Contract.Name)
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
                        _registryData[++_registryCount]    = new Registry(hash, type, data.Manager);
                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position     = _registryCount;
                        _version += 1;
                    }
                }
            }
        }

        protected virtual void RegisterContracts(in RegistrationData data, ref Identity identity)
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
                        _registryData[++_registryCount]    = new Registry(hash, type, identity.Name, data.Manager);
                        _registryMeta[_registryCount].Next = _registryMeta[bucket].Position;
                        _registryMeta[bucket].Position     = _registryCount;
                        references[++referenceCount]       = _registryCount;
                        _version += 1;
                    }
                }

                // Record new count after all done
                references[0] = referenceCount;
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

            for (var current = START_INDEX; current <= _registryCount; current++)
            {
                var bucket = _registryData[current].Hash % size;
                _registryMeta[current].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = current;
            }
        }

        #endregion
    }
}
