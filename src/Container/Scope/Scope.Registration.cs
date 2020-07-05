using System;
using System.Threading.Tasks;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        public bool IsExplicitlyRegistered(Type type, string? name)
        {
            return false;
        }


        #region Register

        public virtual void Register(Type type, string? name, RegistrationManager manager)
        {
            int position;
            var nameHash = name?.GetHashCode() ?? 0;

            // Register contract
            lock (_registryData)
            {
                var hashCode = Registry.GetHashCode(type, nameHash);
                var targetBucket = hashCode % _registryMeta.Length;
                for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                {
                    ref var candidate = ref _registryData[i];
                    if (candidate.Type != type || candidate.Name != name) continue;

                    candidate.Manager = manager;
                    return; // TODO: Dispose manager
                }

                // Expand if required
                if (_registryCount >= _registryMax)
                {
                    ExpandRegistry();
                    targetBucket = hashCode % _registryMeta.Length;
                }

                ref var entry  = ref _registryData[++_registryCount];
                entry.HashCode = hashCode;
                entry.Type     = type;
                entry.Name     = name;
                entry.Manager  = manager;

                _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                _registryMeta[targetBucket].Bucket = _registryCount;
                
                position = _registryCount;
            }

            // Done if anonymous type
            if (null == name) return;

            // Get metadata element
            ref Identity identity = ref _identityData[0];
            lock (_identityData)
            {
                bool found = false;
                var hashCode = nameHash & HashMask;
                var targetBucket = hashCode % _identityMeta.Length;
                for (var i = _identityMeta[targetBucket].Bucket; i > 0; i = _identityMeta[i].Next)
                {
                    identity = ref _identityData[i];
                    if (identity.Name == name)
                    { 
                        // Existing name
                        found = true;
                        break;
                    }
                }

                // Add name to the list
                if (!found)
                { 
                    // Expand if required
                    if (_identityCount >= _identityMax)
                    {
                        ExpandIdentity();
                        targetBucket = hashCode % _identityMeta.Length;
                    }

                    identity = ref _identityData[++_identityCount];
                    identity.Name       = name;
                    identity.HashCode   = hashCode;
                    identity.References = new int[IDENTITY_SIZE];

                    _identityMeta[_identityCount].Next = _identityMeta[targetBucket].Bucket;
                    _identityMeta[targetBucket].Bucket = _identityCount;
                }
            }

            // Add registration for the name element
            lock (identity.Name)
            {
                if (identity.Count >= identity.References.Length)
                    Array.Resize(ref identity.References, identity.Count + IDENTITY_SIZE);

                identity.References[identity.Count++] = position;
            }
        }

        public virtual void Register(Type[] type, string? name, RegistrationManager manager)
        {

        }

        #endregion


        #region Register Asynchronously

        public virtual Task RegisterAsync(Type type, string? name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type[] type, string? name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        #endregion
    }
}
