using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal struct ScopeSet
        {
            #region Fields

            private Metadata[] _meta;
            private Metadata[] _data;
            private readonly Scope[] _ancestry;

            #endregion


            #region Constructors

            public ScopeSet(Scope[] ancestry)
            {
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
                    ref var entry = ref _ancestry[local.Location]
                                            .Data[local.Position]
                                            .Internal.Contract;
                    if (ReferenceEquals(contract.Type, entry.Type) && 
                                        contract.Name == entry.Name)
                        return true;

                    position = _meta[position].Location;
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
                        ref var entry = ref _ancestry[local.Location]
                                                     [local.Position].Internal
                                                                     .Contract;
                        if (ReferenceEquals(entry.Type, contract.Type) && 
                                            entry.Name == contract.Name)
                            return false;

                        position = _meta[position].Location;
                    }
                }

                var count = _data.Increment();
                if (_data.Length <= count)
                {
                    Expand();
                    target = ((uint)contract.HashCode) % _meta.Length;
                }

                // Add new registration
                ref var bucket = ref _meta[target];
                _data[count] = iterator.Location;
                _meta[count].Location = bucket.Position;
                bucket.Position = count;

                return true;
            }

            public Metadata[] GetLocations() => _data;

            #endregion

            private void Expand()
            {
                var prime = Storage.Prime.IndexOf(_meta.Length);
                var meta  = new Metadata[Storage.Prime.Numbers[++prime]];
                var count = _data.Count();

                for (var position = 1; position < count; position++)
                {
                    ref var record = ref _data[position];
                        var scope  = _ancestry[record.Location];
                    ref var contract = ref scope[record.Position].Internal.Contract;

                    var bucket = ((uint)contract.HashCode) % meta.Length;
                    meta[position].Location = meta[bucket].Position;
                    meta[bucket].Position = position;
                }

                _data.CopyTo(_meta, 0);
                _data = _meta; 
                _meta = meta;
            }
        }
    }
}
