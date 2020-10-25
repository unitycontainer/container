using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal struct ScopeSet
        {
            #region Fields

            private Metadata[] _meta;
            public Metadata[] _data;
            private readonly Scope[] _ancestry;

            #endregion


            #region Constructors

            public ScopeSet(Scope[] ancestry)
            {
                _ancestry = ancestry;

                int total = 0;
                for (var i = 0; i < ancestry.Length; i++) total += ancestry[i].Count;
                var prime = Storage.Prime.IndexOf((int)(total * LoadFactor));
                
                _data = new Metadata[Storage.Prime.Numbers[prime++]];
                _meta = new Metadata[Storage.Prime.Numbers[prime]];
            }

            public ScopeSet(Scope scope)
            {
                _ancestry = scope.Ancestry;

                int total = 0;
                for (var i = 0; i < _ancestry.Length; i++) total += _ancestry[i].Count;
                var prime = Storage.Prime.IndexOf((int)(total * LoadFactor));

                _data = new Metadata[Storage.Prime.Numbers[prime++]];
                _meta = new Metadata[Storage.Prime.Numbers[prime]];
                _data.Version(scope.Version);
            }

            #endregion


            #region API

            public bool Contains(in Contract contract)
            {
                var position = _meta[((uint)contract.HashCode) % _meta.Length].Position;

                while (position > 0)
                {
                    ref var record = ref _data[position];
                    ref var entry  = ref _ancestry[record.Location]
                                                  [record.Position].Internal.Contract;
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
                        ref var record = ref _data[position];
                        ref var entry  = ref _ancestry[record.Location]
                                                      [record.Position].Internal.Contract;

                        if (ReferenceEquals(entry.Type, contract.Type) && entry.Name == contract.Name)
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


            public bool Add(in Enumerator enumerator)
            {
                ref var contract = ref enumerator.Internal.Contract;
                var target = ((uint)contract.HashCode) % _meta.Length;

                if (null != contract.Name)
                {
                    var position = _meta[target].Position;

                    while (position > 0)
                    {
                        ref var record = ref _data[position];
                        ref var entry = ref _ancestry[record.Location]
                                                      [record.Position].Internal.Contract;

                        if (ReferenceEquals(entry.Type, contract.Type) && entry.Name == contract.Name)
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
                _data[count] = enumerator.Location;
                _meta[count].Location = bucket.Position;
                bucket.Position = count;

                return true;
            }

            #endregion

            private void Expand()
            {
                var count = _data.Count();
                var prime = Storage.Prime.IndexOf(_meta.Length);
                var meta  = new Metadata[Storage.Prime.Numbers[++prime]];

                for (var position = 1; position < count; position++)
                {
                    ref var record = ref _data[position];
                    ref var bucket = ref meta[((uint)_ancestry[record.Location][record.Position]
                                            .Internal.Contract.HashCode) % meta.Length];

                    meta[position].Location = bucket.Position;
                    bucket.Position = position;
                }

                _data.CopyTo(_meta, 0);
                _data = _meta; 
                _meta = meta;
            }
        }
    }
}
