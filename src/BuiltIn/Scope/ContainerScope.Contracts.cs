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
            RunningIndex++;
            _contractData[RunningIndex] = new ContainerRegistration(in contract, manager);
            _contractMeta[RunningIndex].Next = bucket.Position;
            bucket.Position = (int)RunningIndex;
            _version += 1;

            return (int)RunningIndex;
        }

        protected override void Expand(long required)
        {
            var size = Prime.GetNext((int)(required * ReLoadFactor));

            _contractMeta = new Metadata[size];
            _contractMeta.Setup(LoadFactor);

            base.Expand(_contractMeta.Capacity());

            for (var current = START_INDEX; current <= RunningIndex; current++)
            {
                var bucket = (uint)_contractData[current]._contract.HashCode % size;
                _contractMeta[current].Next = _contractMeta[bucket].Position;
                _contractMeta[bucket].Position = current;
            }
        }
    }
}
