using System;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Utility
{
    internal static class RegistryExtensions
    {
        #region Set

        internal static void Set(this Registry<IPolicySet> registry, Type type, IPolicySet set)
        {
            var hashCode = type.GetHashCode();
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Type != type || candidate.Value is ImplicitRegistration)
                    continue;

                candidate.Value = set;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Type = type;
            entry.Value = set;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<IPolicySet> registry, Type type, string? name, IPolicySet policies)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = (hashCode & UnityContainer.HashMask) % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Type != type ||
                  !(candidate.Value is ImplicitRegistration registration) || registration.Name != name)
                    continue;

                candidate.Value = policies;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Type = type;
            entry.Value = policies;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        #endregion
    }
}
