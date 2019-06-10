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
            bool adding = true;
            int position = 0;
            var collisions = 0;

            LifetimeManager? manager = null;
            ResolveDelegate<BuilderContext>? pipeline = null;

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
                    manager = Context.TypeLifetimeManager.CreateLifetimePolicy();
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
                    manager = factory.LifetimeManager.CreateLifetimePolicy();
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;

                    // Create new entry
                    ref var entry = ref _registry.Entries[_registry.Count];
                    entry.Key = key;
                    entry.Pipeline = manager.Pipeline;
                    entry.Next = _registry.Buckets[targetBucket];
                    position = _registry.Count++;
                    _registry.Buckets[targetBucket] = position;
                }

                if (manager is IDisposable disposable) LifetimeContainer.Add(disposable);
            }

            Debug.Assert(null != manager);

            lock (manager)
            {
                if ((Delegate)(ResolveDelegate<BuilderContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(key.Type, factory, manager, this);
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
