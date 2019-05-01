using System;
using Unity.Storage;

namespace Unity.Extensions
{
    internal static class MetadataExtensions
    {
        internal static int GetEntries<TElement>(this Registry<int[]> metadata, int hashCode, out int[]? data)
        {
            var targetBucket = (hashCode & UnityContainer.HashMask) % metadata.Buckets.Length;

            // Check if metadata exists
            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].HashCode != hashCode ||
                    metadata.Entries[i].Type != typeof(TElement))
                {
                    continue;
                }

                // Get a fix on the buffer
                data = metadata.Entries[i].Value;
                return data[0];
            }

            // Nothing is found
            data = null;
            return 0;
        }

        internal static int GetEntries(this Registry<int[]> metadata, int hashCode, Type type, out int[]? data)
        {
            var targetBucket = (hashCode & UnityContainer.HashMask) % metadata.Buckets.Length;

            // Check if metadata exists
            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].HashCode != hashCode ||
                    metadata.Entries[i].Type != type) continue;

                // Get a fix on the buffer
                data = metadata.Entries[i].Value;
                return data[0];
            }

            // Nothing is found
            data = null;
            return 0;
        }

        public static bool Contains(this Registry<int[]> metadata, int hashCode, Type type)
        {
            var targetBucket = (hashCode & UnityContainer.HashMask) % metadata.Buckets.Length;

            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].HashCode == hashCode &&
                    metadata.Entries[i].Type == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
