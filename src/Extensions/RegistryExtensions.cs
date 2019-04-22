using System;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Extensions
{
    internal static class RegistryExtensions
    {
        #region Get

        public static IPolicySet Get(this Registry<NamedType, IPolicySet> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

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

        internal static void Set(this Registry<NamedType, IPolicySet> registry, Type type, ImplicitRegistration registration)
        {
            var hashCode = type.GetHashCode() & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key.Type != type) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<NamedType, IPolicySet> registry, Type type, string name, ImplicitRegistration registration)
        {
            var hashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key.Type != type || candidate.Key.Name != name) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Key.Name = name;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        #endregion


        #region Contains

        public static bool Contains(this Registry<NamedType, IPolicySet> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

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
