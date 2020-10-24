using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal struct ScopeSet
        {
            #region Fields

            private int _count;
            private Metadata[] _meta;
            private Metadata[] _data;
            private readonly Scope[] _ancestry;

            #endregion


            #region Constructors

            public ScopeSet(Scope[] ancestry)
            {
                _count = 0;
                _ancestry = ancestry;

                // TODO: var prime = Storage.Prime.IndexOf(7 * ancestry.Length);
                var prime = 0;
                _data = new Metadata[Storage.Prime.Numbers[prime++]];
                _meta = new Metadata[Storage.Prime.Numbers[prime]];
            }

            #endregion


            #region API

            public bool Contains(in Contract contract)
            {
                var position = _meta[((uint)contract.HashCode) % _meta.Length].Position;

                while (position > 0)
                {
                    ref var local = ref _data[position];
                        var scope = _ancestry[local.Reference];
                    ref var entry = ref scope.Data[local.Position].Internal.Contract;

                    if (ReferenceEquals(contract.Type, entry.Type) && 
                                        contract.Name == entry.Name)
                        return true;

                    position = _meta[position].Reference;
                }

                return false;
            }

            public bool Add(in Iterator iterator)
            {
                ref var contract = ref iterator.Internal.Contract;
                    var target = ((uint)contract.HashCode) % _meta.Length;

                if (null != contract.Name)
                {
                    var position = _meta[target].Position;

                    while (position > 0)
                    {
                        ref var local = ref _data[position];
                        ref var entry = ref _ancestry[local.Reference][local.Position].Internal.Contract;

                        if (ReferenceEquals(entry.Type, contract.Type) && 
                                            entry.Name == contract.Name)
                            return false;

                        position = _meta[position].Reference;
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
                _meta[_count].Reference = bucket.Position;
                bucket.Position = _count;

                return true;
            }

            public Metadata[] GetLocations() => _data;

            #endregion

            private void Expand()
            {
                var prime = Storage.Prime.IndexOf(_meta.Length);
                var meta  = new Metadata[Storage.Prime.Numbers[++prime]];

                for (var position = 1; position < _count; position++)
                {
                    ref var record = ref _data[position];
                        var scope  = _ancestry[record.Reference];
                    ref var contract = ref scope[record.Position].Internal.Contract;

                    var bucket = ((uint)contract.HashCode) % meta.Length;
                    meta[position].Reference = meta[bucket].Position;
                    meta[bucket].Position = position;
                }

                Array.Copy(_data, _meta, _data.Length);
                _data = _meta; 
                _meta = meta;
            }
        }
    }
}
