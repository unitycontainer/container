using System;
using System.Diagnostics;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {
            #region Fields

            protected int _registryCount;
            protected int _registryPrime;
            protected int[] _registryBuckets;
            protected RegistryEntry[] _registry;

            #endregion


            protected void Add(Type type, RegistrationManager manager)
            {
                var hashCode = NamedType.GetHashCode(type, null) & HashMask;
                var targetBucket = hashCode % _registryBuckets.Length;

                // Create new entry
                ref var entry = ref _registry[_registryCount];
                entry.HashCode = hashCode;
                entry.Type = type;
                entry.Name = null;
                entry.Manager = manager;
                entry.Next = _registryBuckets[targetBucket];
                _registryBuckets[targetBucket] = _registryCount++;
            }


            #region Registration Entry

            [DebuggerDisplay("{Manager}", Name = "{Type.Name}")]
            public struct RegistryEntry
            {
                public int Next;
                public int HashCode;

                public Type    Type;
                public string? Name;
                public RegistrationManager Manager;
            }

            #endregion
        }
    }
}
