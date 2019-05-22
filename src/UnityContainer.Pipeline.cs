using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Container Resolution

        [SecuritySafeCritical]
        private static ValueTask<object?> ResolveContainerPipeline(ref PipelineContext context) 
            => new ValueTask<object?>(context.ContainerContext.Container);

        #endregion


        private BuildPipelineAsync GetPipeline(Type type, string? name)
        {
            var key = new HashKey(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to the parent if no pipelines
                if (null == container._pipelines) continue;

                var registry = container._pipelines;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found it
                    return candidate.Pipeline;
                }
            }

            // Make compiler happy
            Debug.Assert(null != _root);
            Debug.Assert(null != _root._pipelines);

            // Return default build pipeline
            return _root._pipelines.Entries[0].Pipeline;
        }


        #region Root Build Pipeline

        [SecuritySafeCritical]
        public ValueTask<object?> BuildPipeline(ref PipelineContext context)
        {
            var type = context.Type;
            var name = context.Name;
            var overrides = context.Overrides;
            var container = context.ContainerContext;

            if (context.RunAsync)
            {
                var task = Task.Factory.StartNew(DoThePipeline);
                return new ValueTask<object?>(task);
            }

            return new ValueTask<object?>(DoThePipeline());


            object? DoThePipeline()
            {
                // Get Registration if exists
#if NETSTANDARD1_0 || NETCOREAPP1_0
                var info = type.GetTypeInfo();
                var registration = info.IsGenericType 
                    ? GetGenericExplicitRegistration(type, name, info) 
                    : GetSimpleExplicitRegistration(type, name);
#else
                var registration = type.IsGenericType
                    ? GetGenericExplicitRegistration(type, name)
                    : GetSimpleExplicitRegistration(type, name);
#endif
                var c = new PipelineContext
                {
                    Type = type,
                    Name = name,
                    Overrides = overrides,
                    ContainerContext = container,
                };

                // Build Pipeline
                var builder = new PipelineBuilder(ref c, registration);
                var pipeline = builder.BuildPipelineAsync();

                return pipeline(ref c).Result;
            }
        }

        #endregion


        #region Get Registration 

        private IRegistration? GetSimpleExplicitRegistration(Type type, string? name)
        {
            var key = new HashKey(type, name);

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

                    // Found a registration
                    if (!(candidate.Policies is IRegistration))
                        candidate.Policies = container.CreateRegistration(type, name, candidate.Policies);

                    return (IRegistration)candidate.Policies;
                }
            }

            Debug.Assert(null != _root);

            return null;
        }

#if NETSTANDARD1_0 || NETCOREAPP1_0
        private IRegistration? GetGenericExplicitRegistration(Type type, string? name, TypeInfo info)
#else
        private IRegistration? GetGenericExplicitRegistration(Type type, string? name)
#endif
        {
            bool initGenerics = true;
            Type? generic = null;
            int targetBucket;
            var keyExact = new HashKey(type, name);
            var keyGeneric = new HashKey();
            var keyDefault = new HashKey();

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Check for exact match
                targetBucket = keyExact.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyExact) continue;

                    // Found a registration
                    if (!(candidate.Policies is IRegistration))
                        candidate.Policies = container.CreateRegistration(type, name, candidate.Policies);

                    return (IRegistration)candidate.Policies;
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

                // Check for factory with same name
                targetBucket = keyGeneric.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyGeneric)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Policies);
                }

                // Check for default factory
                targetBucket = keyDefault.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyDefault)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Policies);
                }
            }

            return null;
        }

        #endregion
    }
}
