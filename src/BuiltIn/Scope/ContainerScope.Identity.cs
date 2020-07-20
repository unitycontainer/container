using System;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        protected virtual int IndexOf(uint hash, string? name, int required)
        {
            var length = _identityCount;

            // Check if already registered
            var bucket = hash % _identityMeta.Length;
            var position = _identityMeta[bucket].Position;
            while (position > 0)
            {
                if (_identityData[position].Name == name) return position;
                position = _identityMeta[position].Next;
            }

            lock (_contractSync)
            {
                // Check again if length changed during wait for lock
                if (length != _identityCount)
                {
                    bucket = hash % _identityMeta.Length;
                    position = _identityMeta[bucket].Position;
                    while (position > 0)
                    {
                        if (_identityData[position].Name == name) return position;
                        position = _identityMeta[position].Next;
                    }
                }

                // Expand if required
                if (_identityCount >= _identityMax)
                {
                    var size = Prime.Numbers[++_identityPrime];
                    _identityMax = (int)(size * LoadFactor);

                    Array.Resize(ref _identityData, size);
                    _identityMeta = new Metadata[size];

                    // Rebuild buckets
                    for (var current = START_INDEX; current <= _identityCount; current++)
                    {
                        bucket = _identityData[current].Hash % size;
                        _identityMeta[current].Next = _identityMeta[bucket].Position;
                        _identityMeta[bucket].Position = current;
                    }

                    bucket = hash % _identityMeta.Length;
                }

                _identityData[++_identityCount] = new Identity(hash, name, required + 1);
                _identityMeta[_identityCount].Next = _identityMeta[bucket].Position;
                _identityMeta[bucket].Position = _identityCount;

                return _identityCount;
            }
        }
    }
}
