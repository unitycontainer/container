using System.Diagnostics;
using Unity.Lifetime;

namespace Unity.Storage
{
    /// <summary>
    /// Container holding references to all the objects the <see cref="UnityContainer"/>
    /// is responsible to keep alive.
    /// </summary>
    public class LifetimeContainer : ILifetimeContainer
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _sync;

        private int        _count;
        private Entry[]    _data;
        private Metadata[] _meta;

        #endregion


        #region Constructors

        public LifetimeContainer(int capacity = 5)
        {
            var prime = Prime.IndexOf(capacity);
            
            _sync = new();
#if NETSTANDARD
            _data = new Entry[Prime.Numbers[prime++]];
#else
            _data = GC.AllocateUninitializedArray<Entry>(Prime.Numbers[prime++], false);
#endif
            _meta = new Metadata[Prime.Numbers[prime]];
        }

        #endregion

        public int Count => _count;


        /// <inheritdoc/>
        public void Add(IDisposable item)
        {
            var hash = (uint)item.GetHashCode();

            lock (_sync)
            {
                try
                {
                    ref var bucket = ref _meta[hash % _meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (hash == candidate.Hash && ReferenceEquals(candidate.Value, item))
                        {
                            // Found existing
                            return;
                        }

                        position = _meta[position].Location;
                    }

                    if (++_count >= _data.Length)
                    {
                        var prime = Prime.IndexOf(_meta.Length);

                        Array.Resize(ref _data, Prime.Numbers[prime++]);
                        _meta = new Metadata[Prime.Numbers[prime]];

                        for (var current = 1; current < _count; current++)
                        {
                            var local = _data[current].Hash % _meta.Length;
                            _meta[current].Location = _meta[local].Position;
                            _meta[local].Position = current;
                        }

                        bucket = ref _meta[hash % _meta.Length];
                    }

                    // Add new 
                    ref var entry = ref _data[_count];

                    entry.Hash = hash;
                    entry.Value = item;

                    _meta[_count].Location = bucket.Position;
                    bucket.Position = _count;
                }
                catch 
                { 
                    throw new ObjectDisposedException(GetType().FullName);
                }
            }
        }

        /// <inheritdoc/>
        public bool Contains(IDisposable item)
        {
            try
            {
                var hash = (uint)item.GetHashCode();
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (hash == candidate.Hash && ReferenceEquals(candidate.Value, item))
                    {
                        // Found existing
                        return true;
                    }

                    position = _meta[position].Location;
                }

                return false;
            }
            catch
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <inheritdoc/>
        public void Remove(IDisposable item)
        {
            try
            {
                var hash = (uint)item.GetHashCode();
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (hash == candidate.Hash && ReferenceEquals(candidate.Value, item))
                    {
                        // Found existing
                        candidate.Value = null;
                        candidate.Hash = 0;
                    }

                    position = _meta[position].Location;
                }
            }
            catch
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }


        public void Dispose()
        {
            Entry[] data;
            int count;

            lock (_sync)
            {
                data = _data;
                count = _count;
                
                _count = 0;
                _data = Array.Empty<Entry>();
                _meta = Array.Empty<Metadata>();
            }

            for (var i = count; i > 0 ; i--)
            {
                try
                {
                    data[i].Value?.Dispose();
                }
                catch { /* Ignore */ }
            }
        }


        #region Implementation

        [DebuggerDisplay("Disposable='{Value}'   Hash ='{HashCode}'")]
        private struct Entry
        {
            public uint Hash;
            public IDisposable? Value;
        }

        #endregion
    }
}
