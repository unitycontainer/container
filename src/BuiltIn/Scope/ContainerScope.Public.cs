using System;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Public Members

        public override int Contracts => _registryCount;

        public override int Names => _identityCount;

        #endregion


        #region Registrations

        public override bool IsRegistered(Type type)
        {
            var scope = this;

            do
            {
                var bucket = (uint)type.GetHashCode() % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (null == candidate.Contract.Name && candidate.Contract.Type == type)
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope._parent) != null);

            return false;
        }

        public override bool IsRegistered(Type type, string name)
        {
            var scope = this;

            do
            {
                if (0 == scope._identityCount) continue;

                var hash = type.GetHashCode(name);
                var bucket = hash % scope._registryMeta.Length;
                var position = scope._registryMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Contract.Type == type && candidate.Contract.Name == name) 
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope._parent) != null);

            return false;
        }

        public override void Register(in RegistrationData data)
        {
            if (null == data.Name)
            {
                RegisterAnonymous(in data);
            }
            else
            {
                var nameHash = (uint)data.Name!.GetHashCode();
                // TODO: Optimize
                var nameIndex = IndexOf(nameHash, data.Name, data.RegisterAs.Length);
                ref var identity = ref _identityData[nameIndex];

                RegisterContracts(in data, ref identity);
            }
        }


        #endregion


        #region Child Scope

        public override Scope CreateChildScope() => new ContainerScope(this);

        #endregion
    }
}
