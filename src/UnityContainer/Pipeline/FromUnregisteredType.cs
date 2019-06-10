using System;
using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<BuilderContext> PipelineFromUnregisteredType(ref HashKey key)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            LifetimeManager? manager = null;
            ResolveDelegate<BuilderContext>? pipeline = null;

            lock (_syncRegistry)
            {
                var adding = true;
                var collisions = 0;
                var targetBucket = key.HashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var existing = ref _registry.Entries[i];
                    if (existing.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    // Pipeline already been created
                    if (null != existing.Pipeline) return existing.Pipeline;

                    // Lifetime Manager
                    manager = Context.TypeLifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;
                    
                    // Type has not been registered
                    if (null == existing.Registration) existing.Pipeline = manager.Pipeline; 

                    // Skip to creation part
                    adding = false;
                    break;
                }

                if (adding)
                {
                    // Expand if required
                    if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                    {
                        _registry = new Registry(_registry);
                        targetBucket = key.HashCode % _registry.Buckets.Length;
                    }

                    // Lifetime Manager
                    manager = Context.TypeLifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;

                    // Create new entry
                    ref var entry = ref _registry.Entries[_registry.Count];
                    entry.Key = key;
                    entry.Next = _registry.Buckets[targetBucket];
                    entry.Pipeline = manager.Pipeline;
                    entry.IsExplicit = true;
                    int position = _registry.Count++;
                    _registry.Buckets[targetBucket] = position;
                }
            }

            Debug.Assert(null != manager);

            lock (manager)
            {
                if ((Delegate)(ResolveDelegate<BuilderContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(key.Type, this, Context.TypePipelineCache);
                    manager.PipelineDelegate = builder.Pipeline();
                    pipeline = (ResolveDelegate<BuilderContext>)manager.PipelineDelegate;
                }
            }

            return manager.Pipeline;


            object? SpinWait(ref BuilderContext context)
            {
                while (null == pipeline)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                return pipeline(ref context);
            }
        }
    }
}
