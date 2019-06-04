using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Get Pipeline

        private ResolveDelegate<BuilderContext>? TryGetPipeline(ref HashKey key)
        {
            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found it
                    return candidate.Pipeline;
                }
            }

            return null;
        }

        private ResolveDelegate<BuilderContext> GetPipeline(ref HashKey key)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = key.Type?.GetTypeInfo();
            return null != info && info.IsGenericType 
                ? GenericGetPipeline(ref key, info) 
                : GetNonGenericPipeline(ref key);
#else
            return null != key.Type && key.Type.IsGenericType 
                ? GenericGetPipeline(ref key)
                : GetNonGenericPipeline(ref key);
#endif
        }

        #endregion


        #region Implementation

        private ResolveDelegate<BuilderContext> GetNonGenericPipeline(ref HashKey key)
        {
            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found it 
                    Debug.Assert(null != candidate.Pipeline);
                    return candidate.Pipeline;
                }
            }

            Debug.Assert(null != _root);

            return _root.PipelineFromType(ref key);
        }

#if NETSTANDARD1_0 || NETCOREAPP1_0
        private ResolveDelegate<BuilderContext> GenericGetPipeline(ref HashKey key, TypeInfo info)
        {
            Debug.Assert(null != info);
#else
        private ResolveDelegate<BuilderContext> GenericGetPipeline(ref HashKey key)
        {
#endif
            Debug.Assert(null != key.Type);

            int targetBucket;
            bool initGenerics = true;
            var keyGeneric = new HashKey();
            var keyDefault = new HashKey();
            Type type = key.Type;
            Type? generic = null;
            var name = key.Name;

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Exact match
                targetBucket = key.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    Debug.Assert(null != candidate.Pipeline);
                    
                    // Found a registration
                    return candidate.Pipeline;
                }

                // Generic registrations
                if (initGenerics)
                {
                    initGenerics = false;

#if NETSTANDARD1_0 || NETCOREAPP1_0
                    generic = info.GetGenericTypeDefinition();
#else
                    generic = type.GetGenericTypeDefinition();
#endif
                    keyGeneric = new HashKey(generic, name);
                    keyDefault = new HashKey(generic);
                }

                // Factory with the same name
                targetBucket = keyGeneric.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyGeneric)
                        continue;

                    // Found a factory
                    return container.PipelineFromFactory(ref key, (ExplicitRegistration)candidate.Policies);
                }

                // Default factory
                targetBucket = keyDefault.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyDefault)
                        continue;

                    // Found a factory
                    return container.PipelineFromFactory(ref key, (ExplicitRegistration)candidate.Policies);
                }
            }

            Debug.Assert(null != _root);

            return _root.PipelineFromType(ref key);
        }

        #endregion


        #region Create Pipeline

        private ResolveDelegate<BuilderContext> PipelineFromType(ref HashKey key)
        {
            Debug.Assert(null != key.Type);
            Debug.Assert(null != _registry);

            var type = key.Type;
            var name = key.Name;
            int count = -1;
            var collisions = 0;
            ResolveDelegate<BuilderContext>? pipeline = null;

            lock (_syncLock)
            {
                var targetBucket = key.HashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    if (null == candidate.Pipeline)
                        candidate.Pipeline = buildPipeline;

                    // Replaced registration
                    return candidate.Pipeline;
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
                entry.Type = type;
                entry.IsExplicit = true;
                entry.Pipeline = buildPipeline;
                int position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            return buildPipeline;


            object? buildPipeline(ref BuilderContext context)
            {
                // Wait for right moment
                while (0 != Interlocked.Increment(ref count))
                {
                    Interlocked.Decrement(ref count);
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                try
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, this, Context.TypePipelineCache);
                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);
                    }

                    return pipeline(ref context);
                }
                finally
                {
                    Interlocked.Decrement(ref count);
                }
            };

        }

        private ResolveDelegate<BuilderContext> PipelineFromRegistration(ref HashKey key, ExplicitRegistration registration, int position)
        {
            Debug.Assert(null != _registry);

            var type = key.Type;
            var name = key.Name;
            ResolveDelegate<BuilderContext>? pipeline = null;

            return BuildPipeline;

            object? BuildPipeline(ref BuilderContext context)
            {
                if (null != pipeline) return pipeline(ref context);
                lock (registration)
                {
                    if (null != pipeline) return pipeline(ref context);

                    PipelineBuilder builder = new PipelineBuilder(registration);

                    if (registration.LifetimeManager is LifetimeManager manager)
                    {
                        manager.PipelineDelegate = builder.Pipeline();
                        pipeline = manager.Pipeline;
                    }
                    else
                        pipeline = builder.Pipeline();

                    Debug.Assert(null != pipeline);
                    Debug.Assert(null != _registry);

                    lock (_syncLock)
                    {
                        if (ReferenceEquals(registration, _registry.Entries[position].Policies))
                        {
                            _registry.Entries[position].Pipeline = pipeline;
                        }
                    }

                    // Check if already created and acquire a lock if not
                    if (pipeline.Target is LifetimeManager lifetime)
                    {
                        // Make blocking check for result
                        var value = lifetime.Get(LifetimeContainer);
                        if (LifetimeManager.NoValue != value) return value;
                    }

                    return pipeline(ref context);
                }
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromFactory(ref HashKey key, ExplicitRegistration factory)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            var type = key.Type;
            var name = key.Name;
            var owner = this;

            int count = -1;
            int position = 0;
            var collisions = 0;
            ResolveDelegate<BuilderContext>? pipeline = null;

            // Add Pipeline to the Registry
            lock (_syncLock)
            {
                var targetBucket = key.HashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    Debug.Assert(null != candidate.Pipeline);

                    // Has already been created
                    return candidate.Pipeline;
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
                entry.Type = type;
                entry.Pipeline = BuildPipeline;
                entry.Next = _registry.Buckets[targetBucket];
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            // Return temporary pipeline
            return BuildPipeline;


            // Create pipeline and add to Registry
            object? BuildPipeline(ref BuilderContext context)
            {
                // Wait for right moment
                while (0 != Interlocked.Increment(ref count))
                {
                    Interlocked.Decrement(ref count);
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                try
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, factory, owner);
                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);
                    }

                    return pipeline(ref context);
                }
                finally
                {
                    Interlocked.Decrement(ref count);
                }
            };
        }

        #endregion
    }
}
