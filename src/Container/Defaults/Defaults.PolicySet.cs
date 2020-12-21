using System;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class Defaults : IPolicySet
    {
        #region Constants

        private static uint _resolverHash = (uint)typeof(ResolveDelegate<PipelineContext>).GetHashCode();

        #endregion


        ///<inheritdoc/>
        public void Clear(Type type) => throw new NotSupportedException();

        ///<inheritdoc/>
        public object? Get(Type type)
        {
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = Meta[hash % Meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = Meta[position].Location;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type type, object value)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                    {
                        // Found existing
                        candidate.Value = value;
                        candidate.Handler?.Invoke(value);
                        return;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                Data[Count] = new Policy(hash, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        /// <summary>
        /// Allocates placeholder
        /// </summary>
        /// <param name="type"><see cref="Type"/> of policy</param>
        /// <returns>Position of the element</returns>
        private int Allocate(Type type)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                        throw new InvalidOperationException($"{type.Name} already allocated");

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                Data[Count] = new Policy(hash, type, null);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return Count;
            }
        }
    }
}
