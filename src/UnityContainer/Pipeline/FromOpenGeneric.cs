using System;
using System.Diagnostics;
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
        private ResolveDelegate<BuilderContext> PipelineFromOpenGeneric(ref HashKey key, ExplicitRegistration factory)
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
            ResolveDelegate<BuilderContext> buildPipeline;
            var manager = null != factory.LifetimeManager && !(factory.LifetimeManager is TransientLifetimeManager)
                ? factory.LifetimeManager.CreateLifetimePolicy()
                : null;

            if (null != manager)
            {
                if (manager is IDisposable disposable) Context.Lifetime.Add(disposable);
                if (manager is ContainerControlledLifetimeManager containerControlled) containerControlled.Scope = Context;

                manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)BuildPipeline;
                buildPipeline = manager.Pipeline;
            }
            else
                buildPipeline = BuildPipeline;

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
                entry.Pipeline = buildPipeline;
                entry.Next = _registry.Buckets[targetBucket];
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            // Return temporary pipeline
            return buildPipeline;


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
                        PipelineBuilder builder = new PipelineBuilder(type, factory, manager, owner);
                        pipeline = builder.Pipeline();
                        if (null != manager) manager.PipelineDelegate = pipeline;
                        Debug.Assert(null != pipeline);
                    }

                    var value = pipeline(ref context);
                    manager?.Set(value, LifetimeContainer);
                    return value;
                }
                finally
                {
                    Interlocked.Decrement(ref count);
                }
            };
        }
    }
}
