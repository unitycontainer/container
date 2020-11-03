using System;
using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public struct Enumerator
        {
            #region Constants

            private const float LOAD_FACTOR = 0.15f;

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
                _hash = 0;
                _type = default!;
                _scope = _root = container._scope;
                _position = default;
                _location = default;
                _stack = new Metadata[container._ancestry.Length + 1];

                do { _hash += _scope.Count; }
                while (null != (_scope = _scope.Next!));

                var prime = Prime.IndexOf((int)(_hash * LOAD_FACTOR));
                _data = new Metadata[Prime.Numbers[prime++]];
                _meta = new Metadata[Prime.Numbers[prime]];
                _scope = _root;
            }

            #endregion


            #region Current

            public readonly ref Contract Contract 
                => ref _scope[_position].Internal.Contract;

            internal RegistrationManager? Manager
                => _scope[_position].Internal.Manager;

            public readonly ref ContainerRegistration Registration
                => ref _scope[_position].Registration;

            public Metadata Location 
                => new Metadata(_scope.Level, _position);

            #endregion


            #region IEnumerator

            public bool MoveNext()
            {
                do 
                { 
                    // Next 
                    _position = _anonymous ? _scope.MoveNext(_position, _type) 
                                           : _scope[_position].Next;
                    
                    if (0 < _position) return true;

                    // Level Up
                    var index = ++_stack[0].Location;
                    if (index <= _stack.Count())
                    {
                        ref var next = ref _stack[index];
                        _scope = _root.Ancestry[next.Location];
                        _position = _anonymous ? next.Position : _scope[next.Position].Next;

                        if (0 < _position) return true;
                    }

                    // Switch to named
                    if (_anonymous)
                    {
                        _anonymous = false;
                        _stack[0].Location = 1;
                        ref var next = ref _stack[1];
                        _scope = _root.Ancestry[next.Location];
                        _position = _scope[next.Position].Next;

                        if (0 < _position) return true;
                    }

                    // Load new type
                    do 
                    {  
                        if (!MoveNextType()) return false; 
                    } 
                    while (!RegisterType());

                    InitializeStack();

                    ref var record = ref _stack[1];
                    _position = record.Position;
                    _scope = _root.Ancestry[record.Location];
                    _anonymous = true;

                    if (_anonymous) return true;
                }
                while (true);
            }

            #endregion


            #region Implementation

            private bool RegisterType()
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
                        if (contract.Name is null)
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
