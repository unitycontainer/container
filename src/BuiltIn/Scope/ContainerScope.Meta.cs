using System;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        protected virtual int Add(in Contract contract, RegistrationManager manager)
        {
            var hash = (uint)contract.HashCode;
            ref var bucket = ref _contractMeta[hash % _contractMeta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (candidate._contract.Type == contract.Type && ReferenceEquals(
                    candidate._contract.Name, contract.Name))
                {
                    // Found existing
                    candidate = new ContainerRegistration(in contract, manager);
                    _version += 1;
                    return 0;
                }

                position = _contractMeta[position].Next;
            }

            // Add new registration
            _contractCount++;
            _contractData[_contractCount] = new ContainerRegistration(in contract, manager);
            _contractMeta[_contractCount].Next = bucket.Position;
            bucket.Position = _contractCount;
            _version += 1;

            return _contractCount;
        }

        protected virtual void Expand(long required)
        {
            var size = Prime.GetNext((int)(required * ReLoadFactor));

            _contractMeta = new Metadata[size];
            _contractMeta.Setup(LoadFactor);

            Array.Resize(ref _contractData, _contractMeta.Capacity());

            for (var current = START_INDEX; current <= _contractCount; current++)
            {
                var bucket = (uint)_contractData[current]._contract.HashCode % size;
                _contractMeta[current].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = current;
            }
        }
    }
}
