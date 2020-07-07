using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                for (var index = 0; index < data.RegisterAs.Length; index++)
                {
                    var type = data.RegisterAs[index];
                    
                    // Skip invalid types
                    if (null == type) continue;

                    // Check for existing
                    var hash = (uint)type.GetHashCode();
                    var targetBucket = hash % _registryMeta.Length;
                    var position = _registryMeta[targetBucket].Bucket;
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

                        _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                        _registryMeta[targetBucket].Bucket = _registryCount;
                    }
                }
            }
        }

        protected void RegisterContracts(ref RegistrationData data)
        {
            var identity = IndexOf(data.Name!, data.RegisterAs.Length);
            ref var references = ref _identityData[identity].References;

            lock (_registrySync)
            {
                var count    = references[0];
                var required = data.RegisterAs.Length + count;

                // Expand references if required
                if (required >= references.Length)
                    Array.Resize(ref references, (int)Math.Round(required / LoadFactor) + 1);

                // Expand registry if required
                required = _registryCount + data.RegisterAs.Length;
                if (required >= _registryMax) ExpandRegistry(required);

                // Iterate and register types
                for (var index = 0; index < data.RegisterAs.Length; index++)
                {
                    var type = data.RegisterAs[index];

                    // Skip invalid types
                    if (null == type) continue;

                    // Check for existing
                    var hash = ((uint)type.GetHashCode()) ^ _identityData[identity].Hash;
                    var targetBucket = hash % _registryMeta.Length;
                    var position = _registryMeta[targetBucket].Bucket;
                    while (position > 0)
                    {
                        ref var candidate = ref _registryData[position];
                        if (candidate.Type == type && identity == candidate.Identity)
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
                        entry.Identity = identity;
                        entry.Manager  = data.Manager;

                        _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                        _registryMeta[targetBucket].Bucket = _registryCount;
                        references[++count] = _registryCount;
                    }
                }

                // Record new count after all done
                references[0] = count;
            }

            return;
        }

        #endregion


        #region Register Asynchronously

        public virtual Task RegisterAsync(Type type, RegistrationManager manager)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type[] types, RegistrationManager manager)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type type, string name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type[] type, string name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        #endregion


        #region Identity

        protected int IndexOf(string name, int required = IDENTITY_SIZE)
        {
            var count = _identityCount;
            var length = _identityMeta.Length;

            // Check if already registered
            var hashCode = (uint)name.GetHashCode();
            var targetBucket = hashCode % length;
            for (var index = _identityMeta[targetBucket].Bucket; index > 0; index = _identityMeta[index].Next)
            {
                if (_identityData[index].Name == name)
                    return index;
            }

            lock (_identitySync)
            {
                // Check again if array has changed during wait for lock
                if (length != _identityMeta.Length || count != _identityCount)
                {
                    targetBucket = hashCode % _identityMeta.Length;
                    for (var index = _identityMeta[targetBucket].Bucket; index > 0; index = _identityMeta[index].Next)
                    {
                        if (_identityData[index].Name == name)
                            return index;
                    }
                }

                // Expand if required
                if (_identityCount >= _identityMax)
                {
                    ExpandIdentity();
                    targetBucket = hashCode % _identityMeta.Length;
                }

                ref var entry = ref _identityData[++_identityCount];
                entry.Name = name;
                entry.Hash = hashCode;
                entry.References = new int[required + 1];

                _identityMeta[_identityCount].Next = _identityMeta[targetBucket].Bucket;
                _identityMeta[targetBucket].Bucket = _identityCount;

                return _identityCount;
            }
        }

        #endregion


        #region Expanding

        protected void ExpandIdentity()
        {
            var size = Prime.Numbers[++_identityPrime];
            _identityMax = (int)(size * LoadFactor);

            Array.Resize(ref _identityData, size);
            _identityMeta = new Metadata[size];

            for (var index = START_INDEX; index <= _identityCount; index++)
            {
                var bucket = _identityData[index].Hash % size;
                _identityMeta[index].Next = _identityMeta[bucket].Bucket;
                _identityMeta[bucket].Bucket = index;
            }
        }

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
                _registryMeta[i].Next = _registryMeta[bucket].Bucket;
                _registryMeta[bucket].Bucket = i;
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

        [DebuggerDisplay("Count = {Count}", Name = "{ Name,nq }")]
        public struct Identity
        {
            public uint Hash;
            public string Name;
            public int[] References;
        }

        #endregion
    }
}
