using System;
using Unity.Container;

namespace Unity.Storage
{
    internal ref struct RegistrationSet
    {
        #region Fields

        int _prime;
        int _index;
        Metadata[] _meta;
        Metadata[] _data;

        #endregion

        public RegistrationSet(int capacity)
        {
            _prime = Prime.NextUp(capacity);
            _data = new Metadata[Prime.Numbers[_prime++]];
            _meta = new Metadata[Prime.Numbers[_prime]];
            _index = 0;
        }


        public bool Add(Scope scope, int position, ref Scope.InternalRegistration registration)
        {
            var target = ((uint)registration.Contract.HashCode) % _meta.Length;

            if (null != registration.Contract.Name)
            {
            }


            // Nothing is found, add new and expand if required
            if (_data.Length <= ++_index)
            {
                Expand();
                target = ((uint)registration.Contract.HashCode) % _meta.Length;
            }

            // Clone manager

            ref var bucket = ref _meta[target];
            _data[_index] = new Metadata();
            _meta[_index].Location = bucket.Position;
            bucket.Position = _index;

            return true;
        }

        private void Expand()
        {
            Array.Resize(ref _data, Prime.Numbers[_prime++]);
            var meta = new Metadata[Prime.Numbers[_prime]];

            for (var current = 1; current <= _index; current++)
            {
                //var bucket = ((uint)_data[current].Internal.Contract.HashCode) % meta.Length;
                //meta[current].Next = meta[bucket].Position;
                //meta[bucket].Position = current;
            }

            _meta = meta;
        }

    }
}
