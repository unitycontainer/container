using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public struct ScopeSet
        {
            #region Fields

            private int _count;
            private Metadata[] _meta;
            private Location[] _data;
            private readonly Scope[] _ancestry;

            #endregion


            #region Constructors

            public ScopeSet(Scope[] ancestry)
            {
                _count = 0;
                _ancestry = ancestry;

                // TODO: var prime = Storage.Prime.IndexOf(7 * ancestry.Length);
                var prime = 0;
                _data = new Location[Storage.Prime.Numbers[prime++]];
                _meta = new Metadata[Storage.Prime.Numbers[prime]];
            }

            #endregion


            #region API

            public bool Contains(in Contract contract)
            {
                var position = _meta[((uint)contract.HashCode) % _meta.Length].Position;

                while (position > 0)
                {
                    var scope = _ancestry[_data[position].Level];
                    ref var entry = ref scope[_data[position].Position].Internal.Contract;

                    if (ReferenceEquals(contract.Type, entry.Type) && contract.Name == entry.Name)
                        return true;

                    position = _meta[position].Next;
                }

                return false;
            }

            internal bool Add(in Iterator iterator)
            {
                ref var contract = ref iterator.Internal.Contract;
                var target = ((uint)contract.HashCode) % _meta.Length;

                if (null != contract.Name)
                {
                    var position = _meta[target].Position;

                    while (position > 0)
                    {
                        ref var metadata = ref _data[position];
                        ref var candidate = ref _ancestry[metadata.Level][metadata.Position].Internal.Contract;

                        if (ReferenceEquals(candidate.Type, contract.Type) && candidate.Name == contract.Name)
                            return false;

                        position = _meta[position].Next;
                    }
                }

                if (_data.Length <= ++_count)
                {
                    Expand();
                    target = ((uint)contract.HashCode) % _meta.Length;
                }

                // Add new registration
                ref var bucket = ref _meta[target];
                _data[_count] = iterator.Location;
                _meta[_count].Next = bucket.Position;
                bucket.Position = _count;

                return true;
            }

            #endregion

            private void Expand()
            {
                int prime = Storage.Prime.IndexOf(_count);

                Array.Resize(ref _data, Storage.Prime.Numbers[++prime]);
                _meta = new Metadata[Storage.Prime.Numbers[++prime]];

                for (var position = 1; position < _count; position++)
                {
                    ref var record = ref _data[position];
                    var scope = _ancestry[record.Level];
                    ref var contract = ref scope[record.Position].Internal.Contract;

                    var bucket = ((uint)contract.HashCode) % _meta.Length;
                    _meta[position].Next = _meta[bucket].Position;
                    _meta[bucket].Position = position;
                }
            }
        }
    }
}
