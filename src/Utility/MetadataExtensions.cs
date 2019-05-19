using System;
using Unity.Storage;

namespace Unity.Utility
{
    internal static class MetadataExtensions
    {
        internal static int GetEntries(this Registry<int[]> metadata, ref HashKey key, out int[]? data)
        {
            var targetBucket = key.HashCode % metadata.Buckets.Length;

            // Check if metadata exists
            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].Key != key) continue;

                // Get a fix on the buffer
                data = metadata.Entries[i].Value;
                return data[0];
            }

            // Nothing is found
            data = null;
            return 0;
        }
    }
}
