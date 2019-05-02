using System;
using Unity.Extensions;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private const int CollisionsCutPoint = 5;

        #endregion


        #region Fields

        private Registry<IPolicySet>? _registry;
        private Registry<int[]>?      _metadata;

        private  IPolicySet _validators;

        private Func<Type, string?, ImplicitRegistration, ImplicitRegistration?> Register;

        #endregion


        #region Defaults

        #pragma warning disable CS8602

        internal DefaultPolicies Defaults => (DefaultPolicies)_root._registry.Entries[0].Value;
        
        #pragma warning restore CS8602

        #endregion


        #region Policy manipulation

        internal object? GetPolicy(Type type, Type policyInterface)
        {
            var hashCode = type?.GetHashCode() ?? 0;

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
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

        internal object? GetPolicy(Type type, string? name, Type policyInterface)
        {
            var hashCode = NamedType.GetHashCode(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
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

        private void SetPolicy(Type type, Type policyInterface, object policy)
        {
            var hashCode = type?.GetHashCode() ?? 0;

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<IPolicySet>();

                // Check for the existing 
                var targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<IPolicySet>(_registry);
                    targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new PolicySet(this, policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        private void SetPolicy(Type type, string? name, Type policyInterface, object policy)
        {
            var hashCode = NamedType.GetHashCode(type, name);

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<IPolicySet>();

                var targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<IPolicySet>(_registry);
                    targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new PolicySet(this, policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        #endregion
    }
}
