using System;
using System.Threading.Tasks;

namespace Unity.Container
{
    public partial class ContainerScope
    {
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
                    var found = false;
                    var type = types[index];
                    if (null == type) continue;
                    var hashCode = type.GetHashCode() & HashMask;
                    var targetBucket = hashCode % _registryMeta.Length;
                    for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                    {
                        ref var candidate = ref _registryData[i];
                        if (candidate.Type != type || null != candidate.Name) continue;

                        found = true;
                        ReplaceManager(ref candidate, manager);
                        break; 
                    }

                    if (!found)
                    { 
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

            // Add registration for the name element
            var index = IndexOf(nameHash & HashMask, name);
            lock (_identityData[index].Name)
            {
                var references = _identityData[index].References;
                var count = references[0];

                if (count + 1 >= references.Length)
                    Array.Resize(ref references, count + IDENTITY_SIZE);

                references[++count] = position;
                references[0] = count;
            }
        }

        public virtual void Register(Type[] types, string name, RegistrationManager manager)
        {
            var nameHash = name.GetHashCode();
            var offset = IndexOf(nameHash & HashMask, name);

            lock (_identityData[offset].Name)
            {
                ref var references = ref _identityData[offset].References;

                // Expand if required
                var count = references[0];
                var required = count + types.Length + 1;
                if (required >= references.Length) 
                    Array.Resize(ref references, required + IDENTITY_SIZE);

                // Register types
                lock (_registrySync)
                {
                    // Expand if required
                    required = _registryCount + types.Length;
                    if (required >= _registryMax) ExpandRegistry(required);

                    for (var index = 0; index < types.Length; index++)
                    {
                        var found = false;
                        var type = types[index];
                        if (null == type) continue;
                        var hashCode = type.GetHashCode() & HashMask;
                        var targetBucket = hashCode % _registryMeta.Length;
                        for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                        {
                            ref var candidate = ref _registryData[i];
                            if (candidate.Type != type || name != candidate.Name) continue;

                            found = true;
                            ReplaceManager(ref candidate, manager);
                            break;
                        }

                        if (!found)
                        {
                            ref var entry = ref _registryData[++_registryCount];
                            entry.Type = type;
                            entry.Name = name;
                            entry.HashCode = hashCode;
                            entry.Manager = manager;

                            _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                            _registryMeta[targetBucket].Bucket = _registryCount;

                            references[++count] = _registryCount;
                        }
                    }
                    references[0] = count;
                }
            }
        }

        #endregion


        #region Register Asynchronously

        public virtual Task RegisterAsync(Type type, RegistrationManager manager)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type[] types, RegistrationManager manager)
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type type, string name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        public virtual Task RegisterAsync(Type[] type, string name, RegistrationManager manager) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        #endregion
    }
}
