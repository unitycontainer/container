using System;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Local policy manipulation

        internal object GetPolicy(Type type, Type policyInterface)
        {
            var hashCode = type.GetHashCode() & HashMask;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                return registration.Get(policyInterface);
            }

            return null;
        }

        internal object GetPolicy(Type type, string name, Type policyInterface)
        {
            var hashCode = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                return registration.Get(policyInterface);
            }

            return null;
        }

        private void SetPolicy(Type type, string name, Type policyInterface, object policy)
        {
            var hashCode = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                registration.Set(policyInterface, policy);
                return;
            }

            throw new InvalidOperationException("No Policy Set to add the policy to");
        }

        private void ClearPolicy(Type type, string name, Type policyInterface)
        {
            var hashCode = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                registration.Clear(policyInterface);
                return;
            }
        }

        #endregion


        #region Query Policy

        internal TPolicy GetPolicy<TPolicy>(Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;

            for (var container = this; null != container; container = container._parent)
            {
                if (null == container._registry) continue;

                var entry = (TPolicy)container._registry.Get(hashCode, type)?
                                                        .Get(typeof(TPolicy));
                if (null != entry) return entry;
            }

            return default;
        }

        internal TPolicy GetPolicy<TPolicy>(Type type, string name)
        {
            var hashExact = NamedType.GetHashCode(type, name) & HashMask;
            var hashNull  = NamedType.GetHashCode(type, null) & HashMask;
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for exact entry
                var policy = container._registry.Get(hashExact, type)?
                                                .Get(typeof(TPolicy));
                if (null != policy) return (TPolicy)policy;

                // Check for 'Cover it All' entry
                policy = container._registry.Get(hashAll, type)?
                                            .Get(typeof(TPolicy));

                if (null != policy) return (TPolicy)policy;

                // Skip if name is 'null'
                if (hashExact == hashNull) continue;
                
                // Check for name 'null' entry
                policy = container._registry.Get(hashNull, type)?
                                            .Get(typeof(TPolicy));

                if (null != policy) return (TPolicy)policy;
            }

            // Nothing found
            return default;
        }

        internal TPolicy GetPolicy<TPolicy>(Type type, Type factoryType, string name)
        {
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;
            var hashTypeFactory = factoryType?.GetHashCode() ?? 0 & HashMask;
            var hashNamedFactory = NamedType.GetHashCode(factoryType, name) & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for 'Catch It All' entry
                var policy = container._registry.Get(hashAll, type)?
                                                .Get(typeof(TPolicy));

                if (null != policy) return (TPolicy)policy;

                // Check for Named factory
                policy = container._registry.Get(hashNamedFactory, factoryType)? 
                                            .Get(typeof(TPolicy));

                if (null != policy) return (TPolicy)policy;

                // Check for 'Catch It All' factory
                policy = container._registry.Get(hashTypeFactory, factoryType)?
                                            .Get(typeof(TPolicy));

                if (null != policy) return (TPolicy)policy;
            }

            // Nothing found
            return default;
        }

        #endregion

    }
}
