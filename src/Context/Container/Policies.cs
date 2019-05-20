using System;
using System.Diagnostics;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext : IPolicyList
        {
            /// <inheritdoc />
            public object? Get(Type policyInterface)
            {
                Debug.Assert(null != Container._root);
                Debug.Assert(null != Container._root._registry);

                var registry = Container._root._registry;
                return registry.Entries[0].Value.Get(policyInterface);
            }

            /// <inheritdoc />
            public object? Get(Type type, Type policyInterface)
            {
                var key = new HashKey(type);

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = key.HashCode % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.Key != key || entry.Value is ImplicitRegistration) continue;

                        return entry.Value.Get(policyInterface);
                    }
                }

                return null;
            }

            /// <inheritdoc />
            public object? Get(Type type, string? name, Type policyInterface)
            {
                var key = new HashKey(type, name);

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = key.HashCode % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.Key != key) continue;

                        return entry.Value.Get(policyInterface);
                    }
                }

                return null;
            }

            /// <inheritdoc />
            public void Set(Type policyInterface, object policy)
            {
                Debug.Assert(null != Container._root);
                Debug.Assert(null != Container._root._registry);

                var registry = Container._root._registry;
                registry.Entries[0].Value.Set(policyInterface, policy);
            }

            /// <inheritdoc />
            public void Set(Type type, Type policyInterface, object policy)
            {
                var key = new HashKey(type);

                lock (Container._syncRegistry)
                {
                    if (null == Container._registry) Container._registry = new Registry<IPolicySet>();

                    // Check for the existing 
                    var targetBucket = key.HashCode % Container._registry.Buckets.Length;
                    for (var i = Container._registry.Buckets[targetBucket]; i >= 0; i = Container._registry.Entries[i].Next)
                    {
                        ref var candidate = ref Container._registry.Entries[i];
                        if (candidate.Key != key || candidate.Value is ImplicitRegistration)
                            continue;

                        candidate.Value.Set(policyInterface, policy);
                        return;
                    }

                    // Expand only if no more space
                    if (Container._registry.Count >= Container._registry.Entries.Length)
                    {
                        Container._registry = new Registry<IPolicySet>(Container._registry);
                        targetBucket = key.HashCode % Container._registry.Buckets.Length;
                    }

                    // Add registration
                    ref var entry = ref Container._registry.Entries[Container._registry.Count];
                    entry.Key = key;
                    entry.Type = type;
                    entry.Next = Container._registry.Buckets[targetBucket];
                    entry.Value = new PolicySet(Container, policyInterface, policy);
                    Container._registry.Buckets[targetBucket] = Container._registry.Count++;
                }
            }

            /// <inheritdoc />
            public void Set(Type type, string? name, Type policyInterface, object policy)
            {
                var key = new HashKey(type, name);

                lock (Container._syncRegistry)
                {
                    if (null == Container._registry) Container._registry = new Registry<IPolicySet>();

                    var targetBucket = key.HashCode % Container._registry.Buckets.Length;

                    // Check for the existing 
                    for (var i = Container._registry.Buckets[targetBucket]; i >= 0; i = Container._registry.Entries[i].Next)
                    {
                        ref var candidate = ref Container._registry.Entries[i];
                        if (candidate.Key != key) continue;

                        candidate.Value.Set(policyInterface, policy);
                        return;
                    }

                    // Expand only if no more space
                    if (Container._registry.Count >= Container._registry.Entries.Length)
                    {
                        Container._registry = new Registry<IPolicySet>(Container._registry);
                        targetBucket = key.HashCode % Container._registry.Buckets.Length;
                    }

                    // Add registration
                    ref var entry = ref Container._registry.Entries[Container._registry.Count];
                    entry.Key = key;
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
                var key = new HashKey(type, name);

                // Iterate through containers hierarchy
                for (UnityContainer? container = Container; null != container; container = container._parent)
                {
                    // Skip to parent if no registrations
                    if (null == container._registry) continue;

                    var registry = container._registry;
                    var targetBucket = key.HashCode % registry.Buckets.Length;
                    for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                    {
                        ref var entry = ref registry.Entries[i];
                        if (entry.Key != key) continue;

                        entry.Value.Clear(policyInterface);
                        return;
                    }
                }
            }
        }
    }
}
