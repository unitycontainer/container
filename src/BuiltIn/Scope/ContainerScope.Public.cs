using System;
using System.Collections.Generic;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Public Members

        /// <summary>
        /// Parent scope
        /// </summary>
        public Scope? Parent => _parent;

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
                    if (candidate.Identity == 0 && candidate.Contract.Type == type)
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
                if (0 == scope._contracts) continue;

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

        public override void Register(ref RegistrationData data)
        {
            if (null == data.Name)
                RegisterAnonymous(ref data);
            else
                RegisterContracts(ref data);
        }

        #endregion


        #region Child Scope

        /// <summary>
        /// Creates new scope for child container
        /// </summary>
        /// <param name="container"><see cref="UnityContainer"/> that owns the scope</param>
        /// <returns>New scope associated with the container</returns>
        public override Scope CreateChildScope(UnityContainer container)
            => new ContainerScope(container);

        #endregion
    }
}
