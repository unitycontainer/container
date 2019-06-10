using System;
using System.Diagnostics;
using System.Threading;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<PipelineContext> PipelineFromTypeFactory(ref HashKey key, UnityContainer container, IPolicySet set)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            LifetimeManager? manager = null;
            ResolveDelegate<PipelineContext>? pipeline = null;
            var typeFactory = (TypeFactoryDelegate)set.Get(typeof(TypeFactoryDelegate));

            // Add Pipeline to the Registry
            lock (_syncRegistry)
            {
                bool adding = true;
                var collisions = 0;
                var targetBucket = key.HashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    // Pipeline already been created
                    if (null != candidate.Pipeline) return candidate.Pipeline;

                    // Lifetime Manager
                    manager = (LifetimeManager)set.Get(typeof(LifetimeManager)) ??
                                               Context.TypeLifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)SpinWait;

                    // Type has not been registered
                    if (null == candidate.Registration) candidate.Pipeline = manager.Pipeline;

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
                    manager = (LifetimeManager)set.Get(typeof(LifetimeManager)) ??
                                               Context.TypeLifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)SpinWait;

                    // Create new entry
                    ref var entry = ref _registry.Entries[_registry.Count];
                    entry.Key = key;
                    entry.Pipeline = manager.Pipeline;
                    entry.Next = _registry.Buckets[targetBucket];
                    _registry.Buckets[targetBucket] = _registry.Count++;
                }
            }

            Debug.Assert(null != manager);

            lock (manager)
            {
                if ((Delegate)(ResolveDelegate<PipelineContext>)SpinWait == manager.PipelineDelegate)
                {
                    manager.PipelineDelegate = typeFactory(key.Type, this);
                    pipeline = (ResolveDelegate<PipelineContext>)manager.PipelineDelegate;
                }
            }

            return manager.Pipeline;



            object? SpinWait(ref PipelineContext context)
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
