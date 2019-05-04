using System;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext : IPolicyList
        {
            /// <inheritdoc />
            public object? Get(Type type, Type policyInterface)
            {
                var hashCode = type?.GetHashCode() ?? 0;

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = (hashCode & HashMask) % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.HashCode != hashCode || entry.Type != type || entry.Value is ImplicitRegistration)
                            continue;

                        return entry.Value.Get(policyInterface);
                    }
                }

                return null;
            }

            /// <inheritdoc />
            public object? Get(Type type, string? name, Type policyInterface)
            {
                var hashCode = NamedType.GetHashCode(type, name);

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = (hashCode & HashMask) % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.HashCode != hashCode || entry.Type != type ||
                          !(entry.Value is ImplicitRegistration set) || set.Name != name)
                            continue;

                        return entry.Value.Get(policyInterface);
                    }
                }

                return null;
            }

            /// <inheritdoc />
            public void Set(Type type, Type policyInterface, object policy)
            {
                var hashCode = type?.GetHashCode() ?? 0;

                lock (Container._syncRegistry)
                {
                    if (null == Container._registry) Container._registry = new Registry<IPolicySet>();

                    // Check for the existing 
                    var targetBucket = (hashCode & HashMask) % Container._registry.Buckets.Length;
                    for (var i = Container._registry.Buckets[targetBucket]; i >= 0; i = Container._registry.Entries[i].Next)
                    {
                        ref var candidate = ref Container._registry.Entries[i];
                        if (candidate.HashCode != hashCode || candidate.Type != type || candidate.Value is ImplicitRegistration)
                            continue;

                        candidate.Value.Set(policyInterface, policy);
                        return;
                    }

                    // Expand only if no more space
                    if (Container._registry.Count >= Container._registry.Entries.Length)
                    {
                        Container._registry = new Registry<IPolicySet>(Container._registry);
                        targetBucket = (hashCode & HashMask) % Container._registry.Buckets.Length;
                    }

                    // Add registration
                    ref var entry = ref Container._registry.Entries[Container._registry.Count];
                    entry.HashCode = hashCode;
                    entry.Type = type;
                    entry.Next = Container._registry.Buckets[targetBucket];
                    entry.Value = new PolicySet(Container, policyInterface, policy);
                    Container._registry.Buckets[targetBucket] = Container._registry.Count++;
                }
            }

            /// <inheritdoc />
            public void Set(Type type, string? name, Type policyInterface, object policy)
            {
                var hashCode = NamedType.GetHashCode(type, name);

                lock (Container._syncRegistry)
                {
                    if (null == Container._registry) Container._registry = new Registry<IPolicySet>();

                    var targetBucket = (hashCode & HashMask) % Container._registry.Buckets.Length;

                    // Check for the existing 
                    for (var i = Container._registry.Buckets[targetBucket]; i >= 0; i = Container._registry.Entries[i].Next)
                    {
                        ref var candidate = ref Container._registry.Entries[i];
                        if (candidate.HashCode != hashCode || candidate.Type != type ||
                          !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                            continue;

                        candidate.Value.Set(policyInterface, policy);
                        return;
                    }

                    // Expand only if no more space
                    if (Container._registry.Count >= Container._registry.Entries.Length)
                    {
                        Container._registry = new Registry<IPolicySet>(Container._registry);
                        targetBucket = (hashCode & HashMask) % Container._registry.Buckets.Length;
                    }

                    // Add registration
                    ref var entry = ref Container._registry.Entries[Container._registry.Count];
                    entry.HashCode = hashCode;
                    entry.Type = type;
                    entry.Next = Container._registry.Buckets[targetBucket];
                    entry.Value = new ImplicitRegistration(Container, name);
                    entry.Value.Set(policyInterface, policy);
                    Container._registry.Buckets[targetBucket] = Container._registry.Count++;
                }
            }

            /// <inheritdoc />
            public void Clear(Type type, string? name, Type policyInterface)
            {
                var hashCode = NamedType.GetHashCode(type, name);

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = (hashCode & HashMask) % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.HashCode != hashCode || entry.Type != type ||
                          !(entry.Value is ImplicitRegistration set) || set.Name != name)
                            continue;

                        entry.Value.Clear(policyInterface);
                        return;
                    }
                }
            }
        }
    }
}
