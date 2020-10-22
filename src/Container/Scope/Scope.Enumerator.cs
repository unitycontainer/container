using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public Enumerator GetEnumerator(int start) => new Enumerator(this, start);


        public struct Enumerator
        {
            #region Fields

            int _start;
            int _index;
            int _count;
           
            Scope    _scope;
            Scope[]  _ancestry;
            Iterator _iterator;

            Metadata[] _meta;
            Metadata[] _data;

            #endregion


            #region Constructors

            public Enumerator(Scope scope, int start)
            {
                _start = start;
                _index = start;
                _count = 0;
                _scope = scope;
                _ancestry = scope._ancestry;
                _iterator = default;

                // Some arbitrary number
                var prime = Storage.Prime.IndexOf(7 * scope._ancestry.Length);
                _data = new Metadata[Storage.Prime.Numbers[prime++]];
                _meta = new Metadata[Storage.Prime.Numbers[prime++]];
            }

            #endregion

            public readonly ref ContainerRegistration Registration => ref _iterator.Registration;

            public void Dispose() { }

            public void Reset() => throw new NotSupportedException();

            public bool MoveNext()
            {
                do
                {
                    if (_iterator.MoveNext()) return true;

                    while (_scope.Count >= ++_index)
                    {
                        ref var candidate = ref _scope.Data[_index].Internal;
                    
                        // Skip named registrations
                        if (null != candidate.Contract.Name) goto SkipToNext;

                        ref var bucket = ref _meta[((uint)candidate.Contract.HashCode) % _meta.Length];
                        var position = bucket.Position;

                        while (position > 0)
                        {
                            var scope = _ancestry[_data[position].Next];
                            ref var entry = ref scope[_data[position].Position].Internal.Contract;

                            // Skip to next if already have the contract
                            if (entry.HashCode == candidate.Contract.HashCode && 
                                ReferenceEquals(candidate.Contract.Type, entry.Type) && null == entry.Name)
                                goto SkipToNext;

                            position = _meta[position].Next;
                        }

                        if (_data.Length <= ++_count) Expand();

                        _data[_count] = new Metadata(_scope.Level, _count);
                        _meta[_count].Next = bucket.Position;
                        bucket.Position = _count;

                        _iterator = new Iterator(_scope, _index, candidate.Contract.Type, candidate.Contract.HashCode, true);

                        if (!_iterator.MoveNext()) goto SkipToNext;

                        return true;
                        SkipToNext:;
                    }

                    _index    = _start;
                    _iterator = default;
                }
                while (null != (_scope = _scope.Next!));

                return false;
            }

            private void Expand()
            {
                int prime = Storage.Prime.IndexOf(_index);

                Array.Resize(ref _data, Storage.Prime.Numbers[++prime]);
                _meta = new Metadata[Storage.Prime.Numbers[++prime]];

                for (var position = 1; position <= _index; position++)
                {
                    ref var record = ref _data[position];
                    var scope = _ancestry[record.Next];
                    ref var contract = ref scope[record.Position].Internal.Contract;
                    if (null == contract.Name) continue;

                    var bucket = ((uint)contract.HashCode) % _meta.Length;
                    _meta[position].Next = _meta[bucket].Position;
                    _meta[bucket].Position = position;
                }
            }

        }
    }
}
