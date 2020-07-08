using System;

namespace Unity.Container
{
    public partial class ContainerScope
    {
        #region Public Members

        /// <summary>
        /// Parent scope
        /// </summary>
        public readonly ContainerScope? Parent;

        /// <summary>
        /// Owner container
        /// </summary>
        public readonly UnityContainer Container;

        #endregion


        #region Registrations

        public bool IsRegistered(Type type)
        {
            var scope = this;

            do
            {
                var hash = (uint)type.GetHashCode();
                var targetBucket = hash % scope._registryMeta.Length;
                var position = scope._registryMeta[targetBucket].Bucket;
                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Identity == 0 && candidate.Type == type)
                        return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = scope.Parent) != null);

            return false;
        }

        public bool IsRegistered(Type type, string name)
        {
            var scope = this;

            do
            {
                var targetBucket = (uint)name.GetHashCode() % scope._identityMeta.Length;
                var identity = scope._identityMeta[targetBucket].Bucket;

                while (identity > 0)
                {
                    if (scope._identityData[identity].Name == name) break;
                    identity = scope._identityMeta[identity].Next;
                }

                if (0 == identity) continue;

                var hash = ((uint)type.GetHashCode()) ^ scope._identityData[identity].Hash;
                targetBucket = hash % scope._registryMeta.Length;
                var position = scope._registryMeta[targetBucket].Bucket;
                while (position > 0)
                {
                    ref var candidate = ref scope._registryData[position];
                    if (candidate.Identity == identity &&
                        candidate.Type == type) return true;

                    position = scope._registryMeta[position].Next;
                }

            } while ((scope = scope.Parent) != null);

            return false;
        }

        public virtual void Register(ref RegistrationData data)
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
        public virtual ContainerScope CreateChildScope(UnityContainer container)
            => new ContainerScope(container);

        #endregion
    }
}
