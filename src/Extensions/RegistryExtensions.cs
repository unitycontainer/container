using System;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Extensions
{
    internal static class RegistryExtensions
    {
        #region Get

        public static IPolicySet? Get(this Registry<NamedType, IPolicySet> registry, int hashCode, Type? type)
        {
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.HashCode != hashCode || entry.Key.Type != type) continue;
                return entry.Value;
            }

            return null;
        }

        #endregion


        #region Set

        internal static void Set(this Registry<NamedType, IPolicySet> registry, Type type, IPolicySet set)
        {
            var hashCode = type.GetHashCode();
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key.Type != type) continue;
                candidate.Value = set;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Value = set;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<NamedType, IPolicySet> registry, Type type, string? name, IPolicySet set)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key.Type != type || candidate.Key.Name != name) continue;
                candidate.Value = set;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Key.Name = name;
            entry.Value = set;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        #endregion


        #region Contains

        public static bool Contains(this Registry<NamedType, IPolicySet> registry, int hashCode, Type type)
        {
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                if (registry.Entries[i].HashCode == hashCode &&
                    registry.Entries[i].Key.Type == type)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
