using System;
using Unity.Container;
using Unity.Storage;
using static Unity.Container.Scope;

namespace Unity
{
    public partial class UnityContainer
    {

        public struct Enumerator
        {
            #region Constants

            private const float LOAD_FACTOR = 0.45f;

            #endregion


            #region Fields

            private bool _anonymous;
            private int _position;
            private int _hash;
            private Type _type;
            private Scope _scope;
            private Metadata[] _meta;
            private Metadata[] _data;
            private Metadata _location;
            private readonly Scope _root;
            private readonly Metadata[] _stack;

            #endregion


            #region Constructor

            public Enumerator(UnityContainer container)
            {
                _anonymous = true;
                _type = default!;
                _scope = _root = container._scope;
                _position = default;
                _location = default;
                var scope = _scope;
                var length = _hash = 0;
                do { length += scope.Count; } while (null != (scope = scope.Next));
                var prime = Prime.IndexOf((int)(length * LOAD_FACTOR));

                _data = new Metadata[Prime.Numbers[prime++]];
                _meta = new Metadata[Prime.Numbers[prime]];
                _stack = new Metadata[container._depth + 2];
            }

            #endregion


            #region Data

            public readonly ref Contract Contract => ref _scope[_position].Internal.Contract;

            internal readonly ref InternalRegistration Internal
                => ref _scope[_position].Internal;

            public readonly ref ContainerRegistration Registration
                => ref _scope[_position].Registration;

            #endregion


            public bool MoveNext()
            {
                do 
                { 
                    // Very first iteration
                    while (0 >= _position)
                    {
                        if (!MoveNextType()) return false;
                        if (!SaveNextType()) continue;

                        InitializeStack();

                        ref var record = ref _stack[1];
                        _position = record.Position;
                        _scope = _root.Ancestry[record.Location];
                        return true;
                    }

                    // Next 
                    _position = _anonymous ? _scope.MoveNext(_position, _type) 
                                           : _scope[_position].Next;
                    if (0 < _position) return true;

                    // Switch to named
                    if (_anonymous)
                    {
                        _position  = _scope[_stack[_stack[0].Location].Position].Next;
                        if (0 < _position)
                        { 
                            _anonymous = false;
                            return true;
                        }
                    }

                    // Level up
                    _anonymous = true;
                    var index = ++_stack[0].Location;
                    if (index <= _stack.Count())
                    {
                        ref var next = ref _stack[index];
                        _position = next.Position;
                        _scope = _root.Ancestry[next.Location];
                        return true;
                    }
                }
                while (true);
            }

            #region Implementation

            private bool SaveNextType()
            {
                ref var bucket   = ref _meta[((uint)_hash) % _meta.Length];
                    var position = bucket.Position;

                while (position > 0)
                {
                    ref var record = ref _data[position];
                    ref var entry = ref _root[in record].Internal.Contract;

                    if (ReferenceEquals(entry.Type, _type) && null == entry.Name)
                        return false;

                    position = _meta[position].Location;
                }

                var count = _data.Increment();
                if (_data.Length <= count)
                {
                    Expand();
                    bucket = ref _meta[((uint)_hash) % _meta.Length];
                }

                // Add new registration
                _data[count] = _location;
                _meta[count].Location = bucket.Position;
                bucket.Position = count;

                return true;
            }

            private void Expand()
            {
                var count = _data.Count();
                var prime = Prime.NextUp(_meta.Length);
                var meta  = new Metadata[Prime.Numbers[prime]];

                for (var position = 1; position < count; position++)
                {
                    ref var record = ref _data[position];
                    ref var bucket = ref meta[((uint)_root[in record].HashCode) % meta.Length];

                    meta[position].Location = bucket.Position;
                    bucket.Position = position;
                }

                _data.CopyTo(_meta, 0);
                _data = _meta;
                _meta = meta;
            }

            private bool MoveNextType()
            {
                var index = _location.Position;
                var scope = 0 == index ? _root : _root.Ancestry[_location.Location];

                do
                {
                    while (++index <= scope.Count)
                    {
                        ref var contract = ref scope[index].Internal.Contract;
                        if (null == contract.Name)
                        {
                            _type = contract.Type;
                            _hash = contract.HashCode;
                            _scope = scope;
                            _location.Location = scope.Level;
                            _location.Position = index;

                            return true;
                        }
                    }

                    index = 0;
                }
                while (null != (scope = scope.Next));

                return false;
            }

            private void InitializeStack()
            {
                var scope = _scope;
                _stack[0] = new Metadata(1, 0);

                do
                {
                    var position = scope.IndexOf(_type, _hash);
                    if (0 < position)
                    {
                        _stack.AddRecord(scope.Level, position);
                        if (0 > _scope[position].Next) return;
                    }
                }
                while (null != (scope = scope.Next));
            }

            #endregion
        }
    }
}
