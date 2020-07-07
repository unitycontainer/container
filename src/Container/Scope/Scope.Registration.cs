using System;
using System.Threading.Tasks;

namespace Unity.Container
{
    public partial class ContainerScope
    {

        public virtual void Register(ref RegistrationData data)
        {
            if (null == data.Name)
                RegisterAnonymous(ref data);
            else
                RegisterContracts(ref data);
        }


        #region Register

        protected virtual void RegisterAnonymous(ref RegistrationData data)
        {
            lock (_registrySync)
            {
                // Expand if required
                var length   = data.RegisterAs.Length;
                var required = _registryCount + length;

                if (required >= _registryMax) ExpandRegistry(required);

                for (var index = 0; index < length; index++)
                {
                    var found = false;
                    var type = data.RegisterAs[index];
                    if (null == type) continue;
                    var hashCode = type.GetHashCode() & HashMask;
                    var targetBucket = hashCode % _registryMeta.Length;
                    for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                    {
                        ref var candidate = ref _registryData[i];
                        if (candidate._type != type || null != candidate.Name) continue;

                        found = true;
                        ReplaceManager(ref candidate, data.Manager);
                        break; 
                    }

                    if (!found)
                    { 
                        ref var entry = ref _registryData[++_registryCount];
                        entry._type = type;
                        entry._name = null;
                        entry._hash = hashCode;
                        entry._manager = data.Manager;

                        _registryMeta[_registryCount].Next = _registryMeta[targetBucket].Bucket;
                        _registryMeta[targetBucket].Bucket = _registryCount;
                    }
                }
            }
        }

        protected virtual void RegisterContracts(ref RegistrationData data)
        {
            var name = data.Name!;
            var nameHash = name.GetHashCode();
            var offset = IndexOf(nameHash & HashMask, name);

            lock (_identityData[offset].Name)
            {
                ref var references = ref _identityData[offset].References;

                // Expand if required
                var count = references[0];
                var length = data.RegisterAs.Length;
                var required = count + length + 1;
                if (required >= references.Length) 
                    Array.Resize(ref references, required + IDENTITY_SIZE);

                // Register types
                lock (_registrySync)
                {
                    // Expand if required
                    required = _registryCount + length;
                    if (required >= _registryMax) ExpandRegistry(required);

                    for (var index = 0; index < length; index++)
                    {
                        var found = false;
                        var type = data.RegisterAs[index];
                        if (null == type) continue;
                        var hashCode = type.GetHashCode() & HashMask;
                        var targetBucket = hashCode % _registryMeta.Length;
                        for (var i = _registryMeta[targetBucket].Bucket; i > 0; i = _registryMeta[i].Next)
                        {
                            ref var candidate = ref _registryData[i];
                            if (candidate._type != type || name != candidate.Name) continue;

                            found = true;
                            ReplaceManager(ref candidate, data.Manager);
                            break;
                        }

                        if (!found)
                        {
                            ref var entry = ref _registryData[++_registryCount];
                            entry._type = type;
                            entry._name = name;
                            entry._hash = hashCode;
                            entry._manager = data.Manager;

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
