using System;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity.Utility
{
    internal static class RegistryExtensions
    {
        #region Set

        internal static void Set(this Registry registry, Type type, IPolicySet set)
        {
            var key = new HashKey(type);
            var targetBucket = key.HashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key != key) continue;

                candidate.Policies = set;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.Key = key;
            entry.Next = registry.Buckets[targetBucket];
            entry.Type = type;
            entry.Policies = set;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry registry, Type type, string? name, ExplicitRegistration registration)
        {
            var key = new HashKey(type, name);
            var targetBucket = key.HashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key != key) continue;

                candidate.Policies = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.Key = key;
            entry.Next = registry.Buckets[targetBucket];
            entry.IsExplicit = true;
            entry.Type = type;
            entry.Policies = registration;
            entry.Registration = new ContainerRegistration(type, registration);
            registry.Buckets[targetBucket] = registry.Count++;
        }

        #endregion
    }
}
