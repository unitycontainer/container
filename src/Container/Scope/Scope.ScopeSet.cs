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

            #endregion

            private void Expand()
            {
                var count = _data.Count();
                var prime = Storage.Prime.NextUp(_meta.Length);
                var meta  = new Metadata[Storage.Prime.Numbers[prime]];

                for (var position = 1; position < count; position++)
                {
                    ref var record = ref _data[position];
                    ref var bucket = ref meta[((uint)_ancestry[record.Location][record.Position].HashCode) % meta.Length];

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
