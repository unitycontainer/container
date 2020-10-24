using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        internal struct Recorder
        {
            #region Fields

            int _index;
            Metadata[] _meta;
            Metadata[] _data;
            readonly UnityContainer[] _ancestry;

            #endregion

            public Recorder(UnityContainer[] ancestry, int capacity = 17)
            {
                var prime = Prime.NextUp(capacity);
                _data = new Metadata[Prime.Numbers[prime++]];
                _meta = new Metadata[Prime.Numbers[prime]];
                _index = 0;
                _ancestry = ancestry;
            }


            public bool Add(in Contract contract)
            {
                Debug.Assert(null == contract.Name);

                ref var bucket = ref _meta[((uint)contract.HashCode) % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var entry = ref _data[position];
                    if (contract.HashCode == entry.Reference && 0 == entry.Position)
                        return false;

                    position = _meta[position].Reference;
                }

                if (_data.Length <= ++_index) Expand();

                _data[_index] = new Metadata(contract.HashCode, 0);
                _meta[_index].Reference = bucket.Position;
                bucket.Position = _index;

                return true;
            }

            public bool Add(int level, int index, in Contract contract)
            {
                if (null == contract.Name)
                {
                    if (_data.Length <= ++_index) Expand();
                    _data[_index] = new Metadata(level, index);
                    return true;
                }

                ref var bucket = ref _meta[((uint)contract.HashCode) % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    var scope = _ancestry[_data[position].Reference]._scope;
                    ref var entry = ref scope[_data[position].Position].Internal.Contract;

                    if (ReferenceEquals(contract.Type, entry.Type) && contract.Name == entry.Name)
                        return false;

                    position = _meta[position].Reference;
                }

                if (_data.Length <= ++_index) Expand();

                _data[_index] = new Metadata(level, index);
                _meta[_index].Reference = bucket.Position;
                bucket.Position = _index;

                return true;
            }

            private void Expand()
            {
                int prime = Prime.NextUp(_meta.Length);

                Array.Resize(ref _data, Prime.Numbers[prime++]);
                _meta = new Metadata[Prime.Numbers[prime]];

                for (var position = 1; position <= _index; position++)
                {
                    ref var record = ref _data[position];
                    var scope = _ancestry[record.Reference]._scope;
                    ref var contract = ref scope[record.Position].Internal.Contract;
                    if (null == contract.Name) continue;

                    var bucket = ((uint)contract.HashCode) % _meta.Length;
                    _meta[position].Reference = _meta[bucket].Position;
                    _meta[bucket].Position = position;
                }
            }
        }
    }
}