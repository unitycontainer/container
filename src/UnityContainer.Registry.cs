using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private Registry _registry;
        private Metadata _metadata;

        #endregion


        #region

        private IPolicySet InitAndAdd(ref NamedType key, InternalRegistration registration)
        {
            lock (_syncRoot)
            {
                if (Register == InitAndAdd)
                {
                    _registry = new Registry();
                    _metadata = new Metadata();

                    Register = AddOrReplace;
                }
            }

            return Register(ref key, registration);
        }

        private IPolicySet AddOrReplace(ref NamedType key, InternalRegistration registration)
        {
            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % _registry.Buckets.Length;
            var collisions = 0;

            lock (_syncRoot)
            {
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key.Type != key.Type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Reference;

                    candidate.Reference = registration;

                    return existing;
                }

                if (_registry.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key = key;
                entry.Reference = registration;
                var position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
                _metadata.Add(key.Type, position);

                return null;
            }
        }


        #endregion
    }
}
