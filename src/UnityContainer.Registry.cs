using System;
using System.Diagnostics;
using Unity.Builder;
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

        private const int CollisionsCutPoint = 5;
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));

        public string All { get; } = "ALL NAMES";

        #endregion


        #region Fields

        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private Registry<NamedType, IPolicySet> _registry;
        private Registry<Type, int[]> _metadata;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<Type, string, ImplicitRegistration, ImplicitRegistration> Register;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //private Func<IEnumerable<Type>, string, InternalRegistration, IEnumerable<InternalRegistration>> RegisterAsync;

        #endregion


        #region Defaults

        internal IPolicySet Defaults => _root._registry.Entries[0].Value;
        private  IPolicySet _validators;

        #endregion


        #region Policy manipulation

        internal object GetPolicy(Type type, Type policyInterface)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;

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
            var hashCode = NamedType.GetHashCode(type, name) & HashMask;

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

        private void SetPolicy(Type type, Type policyInterface, object policy)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<NamedType, IPolicySet>();

                var targetBucket = hashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<NamedType, IPolicySet>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new PolicySet(policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        private void SetPolicy(Type type, string name, Type policyInterface, object policy)
        {
            var hashCode = NamedType.GetHashCode(type, name) & HashMask;

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<NamedType, IPolicySet>();

                var targetBucket = hashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<NamedType, IPolicySet>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new PolicySet(policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        internal ResolveDelegate<BuilderContext> GetResolverPolicy(Type type, string name)
        {
            var hashExact = NamedType.GetHashCode(type, name) & HashMask;
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for exact entry
                var policy = container._registry.Get(hashExact, type)?
                                                .Get(typeof(ResolveDelegate<BuilderContext>));
                if (null != policy) return (ResolveDelegate<BuilderContext>)policy;

                // Check for 'Cover it All' entry
                policy = container._registry.Get(hashAll, type)?
                                            .Get(typeof(ResolveDelegate<BuilderContext>));

                if (null != policy) return (ResolveDelegate<BuilderContext>)policy;
            }

            // Nothing found
            return default;
        }

        internal ResolveDelegateFactory GetFactoryPolicy(Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for 'Cover it All' entry
                var policy = container._registry
                                      .Get(hashCode, type)?
                                      .Get(typeof(ResolveDelegateFactory));

                // Skip to parent if nothing
                if (null != policy) return (ResolveDelegateFactory)policy;
            }

            // Nothing found
            return null;
        }

        internal ResolveDelegateFactory GetFactoryPolicy(Type type, string name)
        {
            var hashExact = NamedType.GetHashCode(type, name) & HashMask;
            var hashNull = NamedType.GetHashCode(type, null) & HashMask;
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for exact entry
                var policy = container._registry.Get(hashExact, type)?
                                                .Get(typeof(ResolveDelegateFactory))
                // Check for 'Cover it All' entry
                          ?? container._registry.Get(hashAll, type)?
                                                .Get(typeof(ResolveDelegateFactory));
                // Return if found
                if (null != policy) return (ResolveDelegateFactory)policy;

                // Skip if name is 'null'
                if (hashExact == hashNull) continue;

                // Check for name 'null' entry
                policy = container._registry.Get(hashNull, type)?
                                            .Get(typeof(ResolveDelegateFactory));

                if (null != policy) return (ResolveDelegateFactory)policy;
            }

            // Nothing found
            return null;
        }

        #endregion
    }
}
