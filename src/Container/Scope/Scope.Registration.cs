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

        public virtual void Register(Type type, RegistrationManager manager)
        {
            lock (_registrySync)
            {
                var hashCode = type.GetHashCode() & HashMask;
                var targetBucket = hashCode % _registryMeta.Length;
                for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                {
                    ref var candidate = ref _registryData[i];
                    if (candidate.Type != type || null != candidate.Name) continue;

                    ReplaceManager(ref candidate, manager);
                    return;
                }

                // Expand if required
                if (_registryCount >= _registryMax)
                {
                    ExpandRegistry();
                    targetBucket = hashCode % _registryMeta.Length;
                }

                ref var entry  = ref _registryData[++_registryCount];
                entry.Type     = type;
                entry.Name     = null;
                entry.HashCode = hashCode;
                entry.Manager  = manager;

                _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                _registryMeta[targetBucket].Bucket = _registryCount;
            }
        }

        public virtual void Register(Type[] types, RegistrationManager manager)
        {
            lock (_registrySync)
            {
                // Expand if required
                var required = _registryCount + types.Length;
                if (required >= _registryMax) ExpandRegistry(required);

                for (var index = 0; index < types.Length; index++)
                {
                    var type = types[index];
                    if (null == type) continue;
                    var hashCode = type.GetHashCode() & HashMask;
                    var targetBucket = hashCode % _registryMeta.Length;
                    for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                    {
                        ref var candidate = ref _registryData[i];
                        if (candidate.Type != type || null != candidate.Name) continue;

                        ReplaceManager(ref candidate, manager);
                        return; 
                    }

                    ref var entry = ref _registryData[++_registryCount];
                    entry.Type = type;
                    entry.Name = null;
                    entry.HashCode = hashCode;
                    entry.Manager = manager;

                    _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                    _registryMeta[targetBucket].Bucket = _registryCount;
                }
            }
        }


        public virtual void Register(Type type, string name, RegistrationManager manager)
        {
            var position = 0;
            var nameHash = name.GetHashCode();

            // Register contract
            lock (_registrySync)
            {
                var hashCode = Registry.GetHashCode(type, nameHash);
                var targetBucket = hashCode % _registryMeta.Length;
                for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                {
                    ref var candidate = ref _registryData[i];
                    if (candidate.Type != type || candidate.Name != name) continue;

                    ReplaceManager(ref candidate, manager);
                    return;
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

            var index = IndexOf(nameHash & HashMask, name);
            ref var identity = ref _identityData[index];

            // Add registration for the name element
            lock (identity.Name)
            {
                if (identity.Count >= identity.References.Length)
                    Array.Resize(ref identity.References, identity.Count + IDENTITY_SIZE);

                identity.References[identity.Count++] = position;
            }
        }

        public virtual void Register(Type[] types, string name, RegistrationManager manager)
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
