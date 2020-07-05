using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        #region Implementation

        private void ExpandIdentity()
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


        private void ExpandRegistry()
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
                => ((Type.GetHashCode() + 37) ^ ((Name?.GetHashCode() ?? 0) + 17)) & HashMask;


            public static int GetHashCode(Type type, int code)
                => ((type.GetHashCode() + 37) ^ (code + 17)) & HashMask;

            public static int GetHashCode(Type type, string? name)
                => ((type.GetHashCode() + 37) ^ ((name?.GetHashCode() ?? 0) + 17)) & HashMask;
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
