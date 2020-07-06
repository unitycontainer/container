using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        protected int IndexOf(int hashCode, string name)
        {
            var count  = _identityCount;
            var length = _identityMeta.Length;

            var targetBucket = hashCode % length;
            for (var index = _identityMeta[targetBucket].Bucket; index > 0; index = _identityMeta[index].Next)
            {
                ref var candidate = ref _identityData[index];
                if (candidate.Name != name) continue;

                return index;
            }

            lock (_identitySync)
            {
                if (length != _identityMeta.Length || count != _identityCount)
                {
                    targetBucket = hashCode % _identityMeta.Length;
                    for (var index = _identityMeta[targetBucket].Bucket; index > 0; index = _identityMeta[index].Next)
                    {
                        ref var candidate = ref _identityData[index];
                        if (candidate.Name != name) continue;

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
                entry.HashCode = hashCode;
                entry.References = new int[IDENTITY_SIZE];

                _identityMeta[_identityCount].Next = _identityMeta[targetBucket].Bucket;
                _identityMeta[targetBucket].Bucket = _identityCount;

                return _identityCount;
            }
        }

        protected void ReplaceManager(ref Registry registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
        
        }

        #region Expand

        protected void ExpandIdentity()
        {
            var size = Prime.Numbers[++_identityPrime];
            _identityMax = (int)(size * LoadFactor);

            Array.Resize(ref _identityData, size);
            _identityMeta = new Metadata[size];

            for (var index = START_INDEX; index <= _identityCount; index++)
            {
                var bucket = _identityData[index].HashCode % size;
                _identityMeta[index].Next = _identityMeta[bucket].Bucket;
                _identityMeta[bucket].Bucket = index;
            }
        }

        protected void ExpandRegistry()
        {
            var size = Prime.Numbers[++_registryPrime];
            _registryMax = (int)(size * LoadFactor);

            Array.Resize(ref _registryData, size);
            _registryMeta = new Metadata[size];

            for (var index = START_INDEX; index <= _registryCount; index++)
            {
                var bucket = _registryData[index].HashCode % size;
                _registryMeta[index].Next = _registryMeta[bucket].Bucket;
                _registryMeta[bucket].Bucket = index;
            }
        }

        protected void ExpandRegistry(int required)
        {
            int size;
            
            do
            {
                size = Prime.Numbers[++_registryPrime];
                _registryMax = (int)(size * LoadFactor);
            }
            while (_registryMax <= required);

            Array.Resize(ref _registryData, size);
            _registryMeta = new Metadata[size];

            for (var index = START_INDEX; index <= _registryCount; index++)
            {
                var bucket = _registryData[index].HashCode % size;
                _registryMeta[index].Next = _registryMeta[bucket].Bucket;
                _registryMeta[bucket].Bucket = index;
            }
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("{ (null == Type ? string.Empty : Name),nq }", Name = "{ (Type?.Name ?? string.Empty),nq }")]
        public struct Registry
        {
            public int HashCode;
            public Type Type;
            public string? Name;
            public RegistrationManager Manager;

            public override int GetHashCode()
            {
                var hashCode = null == Name
                    ? Type.GetHashCode()
                    : (Type.GetHashCode() ^ (Name.GetHashCode() + 17));

                return hashCode & HashMask;
            }

            public static int GetHashCode(Type type, int code)
            {
                var hashCode = 0 == code
                    ? type.GetHashCode()
                    : (type.GetHashCode() ^ (code + 17));

                return hashCode & HashMask;
            }

            public static int GetHashCode(Type type, string? name)
            {
                var hashCode = null == name
                    ? type.GetHashCode()
                    : (type.GetHashCode() ^ (name.GetHashCode() + 17));

                return hashCode & HashMask;
            }
        }

        [DebuggerDisplay("Count = {Count}", Name = "{ Name,nq }")]
        public struct Identity
        {
            public string Name;
            public int HashCode;
            public int Count;
            public int[] References;
        }

        #endregion
    }
}
