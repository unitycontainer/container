using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Get Pipeline

        private ResolveDelegate<PipelineContext>? TryGetPipeline(ref HashKey key)
        {
            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry!;

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

        internal ResolveDelegate<PipelineContext> GetPipeline(Type type, string? name)
        {
            var key = new HashKey(type, name);

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

        internal ResolveDelegate<PipelineContext> GetPipeline(ref HashKey key)
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

        private ResolveDelegate<PipelineContext> GetNonGenericPipeline(ref HashKey key)
        {
            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found it 
                    return candidate.Pipeline ??
                           container.PipelineFromRegistration(key.Type, candidate.Registration, i);
                }
            }

            return _root.PipelineFromUnregisteredType(ref key);
        }

#if NETSTANDARD1_0 || NETCOREAPP1_0
        private ResolveDelegate<PipelineContext> GenericGetPipeline(ref HashKey key, TypeInfo info)
#else
        private ResolveDelegate<PipelineContext> GenericGetPipeline(ref HashKey key)
#endif
        {
            Debug.Assert(null != key.Type);

            int targetBucket;
            bool initGenerics = true;
            var keyGeneric = new HashKey();
            var keyDefault = new HashKey();
            Type type = key.Type!;
            Type? generic = null;
            var name = key.Name;

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Exact match
                targetBucket = key.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found a registration
                    return candidate.Pipeline ??
                           container.PipelineFromRegistration(key.Type, candidate.Registration, i);
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

                // Generic Factory
                targetBucket = keyGeneric.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyGeneric)
                        continue;

                    // Found a generic factory
                    return container.PipelineFromOpenGeneric(ref key, candidate.Registration);
                }

                // Default Factory
                targetBucket = keyDefault.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyDefault)
                        continue;

                    // Found a default
                    return container.PipelineFromTypeFactory(ref key, container, candidate.Policies);
                }
            }

            return _root.PipelineFromUnregisteredType(ref key);
        }

        #endregion


        #region Nested Types

        private delegate ResolveDelegate<PipelineContext> FromEntry(ref Registry.Entry entry);

        private delegate ResolveDelegate<PipelineContext> FromUnregistered(ref HashKey key);
        private delegate ResolveDelegate<PipelineContext> FromRegistration(Type? type, ExplicitRegistration registration, int position);
        private delegate ResolveDelegate<PipelineContext> FromOpenGeneric(ref HashKey key, ExplicitRegistration factory);

        #endregion
    }
}
