using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<BuilderContext> PipelineFromTypeFactory(ref HashKey key, UnityContainer container, TypeFactoryDelegate typeFactory)
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
                        pipeline = typeFactory(type, container);
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
    }
}
