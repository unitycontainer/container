using System;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Utility
{
    internal static class RegistryExtensions
    {
        #region Set

        internal static void Set(this Registry<IPolicySet> registry, Type type, IPolicySet set)
        {
            var key = new HashKey(type);
            var targetBucket = key.HashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (!candidate.Key.Equals(ref key)) continue;

                candidate.Value = set;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.Key = key;
            entry.Next = registry.Buckets[targetBucket];
            entry.Type = type;
            entry.Value = set;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<IPolicySet> registry, Type type, string? name, IPolicySet policies)
        {
            var key = new HashKey(type, name);
            var targetBucket = key.HashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (!candidate.Key.Equals(ref key)) continue;

                candidate.Value = policies;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.Key = key;
            entry.Next = registry.Buckets[targetBucket];
            entry.Type = type;
            entry.Value = policies;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        #endregion
    }
}
