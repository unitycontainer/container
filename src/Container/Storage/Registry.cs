using System;
using System.Collections.Generic;
using System.Text;
using Unity.Policy;

namespace Unity.Container.Storage
{
    internal static class Registry
    {
        private const int ListToHashCutoverPoint = 7;

        public static TValue Get<TKey, TValue>(this IRegistry<TKey, TValue> registry, TKey key)
        {
            return default(TValue);
        }

        public static TValue Set<TKey, TValue>(this IRegistry<TKey, TValue> registry, TKey key, TValue value)
        {
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IRegistry<TKey, TValue> registry, TKey key, Func<TValue> factory)
        {
            return registry is HashRegistry<TKey, TValue> hashRegistry ? GetOrAdd(hashRegistry, key, factory) 
                                                                       : ((ListRegistry)registry).GetOrAdd(key, factory);
        }

        public static TValue SetOrUpdate<TKey, TValue>(this IRegistry<TKey, TValue> registry, TKey key, TValue value)
        {
            return value;
        }




        public static TValue Get<TKey, TValue>(this ListRegistry registry, TKey key)
        {
            return default(TValue);
        }

        public static TValue Set<TKey, TValue>(this ListRegistry registry, TKey key, TValue value)
        {
            return value;
        }

        public static TValue SetOrUpdate<TKey, TValue>(this ListRegistry registry, TKey key, TValue value)
        {
            return value;
        }




        public static TValue Get<TKey, TValue>(this HashRegistry<TKey, TValue> registry, TKey key)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % registry.Buckets.Length;
            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                if (registry.Entries[i].HashCode != hashCode ||
                    Equals(registry.Entries[i].Key, key))
                {
                    continue;
                }

                return registry.Entries[i].Value;
            }

            return default(TValue);
        }

        public static TValue Set<TKey, TValue>(this HashRegistry<TKey, TValue> registry, TKey key, TValue value)
        {
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this HashRegistry<TKey, TValue> registry, TKey key, Func<TValue> factory)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % registry.Buckets.Length;
            var collisions = 0;
            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                var entry = registry.Entries[i];
                if (entry.HashCode != hashCode || Equals(entry.Key, key))
                {
                    collisions++;
                    continue;
                }

                return registry.Entries[i].Value;
            }

            if (registry.RequireToGrow || ListToHashCutoverPoint < collisions)
            {
                registry = new HashRegistry<TKey, TValue>(registry);
                targetBucket = hashCode % registry.Buckets.Length;
            }

            var value = factory();
            registry.Entries[registry.Count].HashCode = hashCode;
            registry.Entries[registry.Count].Next = registry.Buckets[targetBucket];
            registry.Entries[registry.Count].Key = key;
            registry.Entries[registry.Count].Value = value;
            registry.Buckets[targetBucket] = registry.Count;
            registry.Count++;

            return value;
        }

        public static TValue SetOrUpdate<TKey, TValue>(this HashRegistry<TKey, TValue> registry, TKey key, TValue value)
        {
            return value;
        }

    }
}
