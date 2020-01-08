using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Check if registered

        internal bool IsRegistered(Type type)
        {
            var key = new HashKey(type);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                var metadata = container._metadata; ;
                var targetBucket = key.HashCode % metadata.Buckets.Length;

                for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
                {
                    if (metadata.Entries[i].HashKey != key) continue;
                    return true;
                }

                return false;
            }

            return false;
        }

        #endregion


        #region Type Registration

        private LifetimeManager? InitAndAddType(Type type, string? name, ExplicitRegistration registration)
        {
            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry();
                if (null == _metadata)
                {
                    _metadata = new Metadata();

                    RegisterType     = AddOrReplaceType;
                    RegisterInstance = AddOrReplaceInstance;
                }
            }

            return RegisterType(type, name, registration);
        }

        private LifetimeManager? AddOrReplaceType(Type type, string? name, ExplicitRegistration registration)
        {
            var collisions = 0;
            var key = new HashKey(type, name);
            var metaKey = new HashKey(type);

            registration.AddRef();

            // Registry
            lock (_syncRegistry)
            {
                Debug.Assert(null != _registry);
                Debug.Assert(null != _metadata);

                var targetBucket = key.HashCode % _registry!.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    // Swap the registration
                    var existing = candidate.Policies as ExplicitRegistration;
                    var manager = candidate.Manager;

                    if (null == existing)
                    {
                        candidate.IsExplicit = true;
                        registration.Add(candidate.Policies);
                    }
                    candidate.Policies = registration;
                    candidate.Pipeline = registration.Pipeline;
                    candidate.Registration = registration;

                    // Replaced registration
                    return manager;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Next = _registry.Buckets[targetBucket];
                entry.IsExplicit = true;
                entry.Policies = registration;
                entry.Pipeline = registration.Pipeline;
                entry.Manager = registration.LifetimeManager;
                entry.Registration = registration;
                int position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;

                collisions = 0;

                // Metadata
                targetBucket = metaKey.HashCode % _metadata!.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.HashKey != metaKey || candidate.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Expand if required
                    if (candidate.Value.Length == candidate.Value[0])
                    {
                        var source = candidate.Value;
                        candidate.Value = new int[source.Length * 2];
                        Array.Copy(source, candidate.Value, source[0]);
                    }

                    // Add to existing
                    candidate.Value[candidate.Value[0]++] = position;

                    // Nothing to replace
                    return null;
                }

                // Expand if required
                if (_metadata.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _metadata = new Metadata(_metadata);
                    targetBucket = metaKey.HashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var metadata = ref _metadata.Entries[_metadata.Count];
                metadata.Next = _metadata.Buckets[targetBucket];
                metadata.HashKey = metaKey;
                metadata.Type = type;
                metadata.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        #endregion


        #region Instance Registration

        private LifetimeManager? InitAndAddInstance(Type type, string? name, LifetimeManager registration)
        {
            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry();
                if (null == _metadata)
                {
                    _metadata = new Metadata();

                    RegisterType     = AddOrReplaceType;
                    RegisterInstance = AddOrReplaceInstance;
                }
            }

            return RegisterInstance(type, name, registration);
        }

        private LifetimeManager? AddOrReplaceInstance(Type type, string? name, LifetimeManager manager)
        {
            var collisions = 0;
            var key = new HashKey(type, name);
            var metaKey = new HashKey(type);

            // Registry
            lock (_syncRegistry)
            {
                Debug.Assert(null != _registry);
                Debug.Assert(null != _metadata);

                var targetBucket = key.HashCode % _registry!.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    // Swap the registration
                    var existing = candidate.Policies as ExplicitRegistration;
                    var replaced = candidate.Manager;

                    if (null == existing)
                    {
                        // TODO: ????
                        candidate.IsExplicit = true;
                    }

                    candidate.Pipeline = manager.Pipeline;

                    // Replaced registration
                    return replaced;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Next = _registry.Buckets[targetBucket];
                entry.IsExplicit = true;
                entry.Manager = manager;
                entry.Pipeline = manager.Pipeline;
                int position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;

                collisions = 0;

                // Metadata
                targetBucket = metaKey.HashCode % _metadata!.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.HashKey != metaKey || candidate.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Expand if required
                    if (candidate.Value.Length == candidate.Value[0])
                    {
                        var source = candidate.Value;
                        candidate.Value = new int[source.Length * 2];
                        Array.Copy(source, candidate.Value, source[0]);
                    }

                    // Add to existing
                    candidate.Value[candidate.Value[0]++] = position;

                    // Nothing to replace
                    return null;
                }

                // Expand if required
                if (_metadata.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _metadata = new Metadata(_metadata);
                    targetBucket = metaKey.HashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var metadata = ref _metadata.Entries[_metadata.Count];
                metadata.Next = _metadata.Buckets[targetBucket];
                metadata.HashKey = metaKey;
                metadata.Type = type;
                metadata.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        #endregion
    }
}
