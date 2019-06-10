using System;
using System.Diagnostics;
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
            bool AddNew = true;
            int position = 0;
            var collisions = 0;
            LifetimeManager? manager = null;

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

                    // Empty Registration with Policies
                    manager = Context.TypeLifetimeManager.CreateLifetimePolicy();

                    // Type has not been registered
                    if (null == candidate.Registration) candidate.Pipeline = manager.Pipeline;

                    AddNew = false;
                    break;
                }

                if (AddNew)
                {
                    // Expand if required
                    if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                    {
                        _registry = new Registry(_registry);
                        targetBucket = key.HashCode % _registry.Buckets.Length;
                    }

                    // Create a Lifetime Manager
                    manager = factory.LifetimeManager.CreateLifetimePolicy();

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

            PipelineBuilder builder = new PipelineBuilder(key.Type, factory, manager, this);
            manager.PipelineDelegate = builder.Pipeline();

            return manager.Pipeline;
        }
    }
}
