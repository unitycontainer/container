using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private struct RegistrationSet
        {
            #region Fields

            private Metadata[] _meta;
            private Metadata[] _data;
            private readonly Scope _scope;

            #endregion


            #region Constructors

            public RegistrationSet(Scope scope)
            {
                var prime = Prime.IndexOf((int)(scope.Version * Scope.LoadFactor));

                _scope = scope;
                _data = new Metadata[Prime.Numbers[prime++]];
                _meta = new Metadata[Prime.Numbers[prime]];
                _data.Version(_scope.Version);
            }

            public RegistrationSet(Scope scope, int index)
            {
                _scope = scope;
                _data = new Metadata[Prime.Numbers[index++]];
                _meta = new Metadata[Prime.Numbers[index]];
                _data.Version(_scope.Version);
            }

            #endregion


            #region API

            public Metadata[] GetRecording() => _data;

            public bool Add(in Enumerator enumerator)
            {
                ref var contract = ref enumerator.Contract;
                var target = ((uint)contract.HashCode) % _meta.Length;

                var position = _meta[target].Position;

                while (position > 0)
                {
                    ref var record = ref _data[position];
                    ref var entry = ref _scope[in record].Internal.Contract;

                    if (contract.HashCode == entry.HashCode && 
                        contract.Type == entry.Type &&
                        contract.Name == entry.Name)
                        return false;

                    position = _meta[position].Location;
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
                var prime = Prime.NextUp(_meta.Length);
                var meta = new Metadata[Prime.Numbers[prime]];

                for (var position = 1; position < count; position++)
                {
                    ref var record = ref _data[position];
                    ref var bucket = ref meta[((uint)_scope[in record].HashCode) % meta.Length];

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
