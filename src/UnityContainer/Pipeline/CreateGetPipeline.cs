using System;
using System.Diagnostics;
using System.Reflection;
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

        internal ResolveDelegate<BuilderContext> GetPipeline(ref HashKey key)
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

            return _root.PipelineFromUnregisteredType(ref key);
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
                    return container.PipelineFromOpenGeneric(ref key, (ExplicitRegistration)candidate.Policies);
                }

                // Default factory
                targetBucket = keyDefault.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyDefault)
                        continue;

                    // Found a factory
                    var typeFactory = (TypeFactoryDelegate)candidate.Policies.Get(typeof(TypeFactoryDelegate));
                    return container.PipelineFromTypeFactory(ref key, container, typeFactory);
                }
            }

            Debug.Assert(null != _root);

            return _root.PipelineFromUnregisteredType(ref key);
        }

        #endregion


        #region Pipeline Creation

        private ResolveDelegate<BuilderContext> DefaultBuildPipeline(LifetimeManager manager, Func<ResolveDelegate<BuilderContext>?> build)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (manager)
                {
                    if (null == pipeline)
                    {
                        pipeline = build();
                        manager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);
                    }
                }

                return pipeline(ref context);
            };
        }

        #endregion
    }
}
