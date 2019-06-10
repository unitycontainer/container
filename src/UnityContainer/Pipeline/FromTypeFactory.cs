using System;
using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<BuilderContext> PipelineFromTypeFactory(ref HashKey key, UnityContainer container, IPolicySet set)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            var type = key.Type;
            var name = key.Name;
            var owner = this;

            int position = 0;
            var collisions = 0;


            bool adding = true;
            LifetimeManager? manager = null;
            ResolveDelegate<BuilderContext>? pipeline = null;
            var typeFactory = (TypeFactoryDelegate)set.Get(typeof(TypeFactoryDelegate));

            // Add Pipeline to the Registry
            lock (_syncRegistry)
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

                    // Pipeline already been created
                    if (null != candidate.Pipeline) return candidate.Pipeline;

                    // Lifetime Manager
                    manager = (LifetimeManager)set.Get(typeof(LifetimeManager)) ??
                                               Context.TypeLifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;

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
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;

                    // Create new entry
                    ref var entry = ref _registry.Entries[_registry.Count];
                    entry.Key = key;
                    entry.Pipeline = manager.Pipeline;
                    entry.Next = _registry.Buckets[targetBucket];
                    position = _registry.Count++;
                    _registry.Buckets[targetBucket] = position;
                }
            }

            Debug.Assert(null != manager);

            lock (manager)
            {
                if ((Delegate)(ResolveDelegate<BuilderContext>)SpinWait == manager.PipelineDelegate)
                {
                    manager.PipelineDelegate = typeFactory(type, this);
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
