using System;
using System.Diagnostics;
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

            LifetimeManager manager;

            lock (_syncLock)
            {

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

                    // TODO: Check if null pipeline is possible
                    if (null == candidate.Pipeline)
                    {
                        // Create or get Lifetime Manager
                        manager = Context.TypeLifetimeManager.CreateLifetimePolicy();
                        manager.PipelineDelegate = manager switch
                        {
                            TransientLifetimeManager  transient => PipelineFromUnregisteredTypeTransient(key.Type, transient, _registry.Count),
                            PerResolveLifetimeManager peresolve => PipelineFromUnregisteredTypePerResolve(key.Type, peresolve),
                            _ => PipelineFromUnregisteredTypeDefault(key.Type, manager)
                        };

                        candidate.Pipeline = manager.Pipeline;
                    }

                    // Replaced registration
                    return candidate.Pipeline;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create a Lifetime Manager
                manager = Context.TypeLifetimeManager.CreateLifetimePolicy();
                manager.PipelineDelegate = manager switch
                {
                    TransientLifetimeManager  transient => PipelineFromUnregisteredTypeTransient(key.Type, transient, _registry.Count),
                    PerResolveLifetimeManager peresolve => PipelineFromUnregisteredTypePerResolve(key.Type, peresolve),
                    _ => PipelineFromUnregisteredTypeDefault(key.Type, manager)
                };

                Debug.Assert(null != key.Type);

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Next = _registry.Buckets[targetBucket];
                entry.IsExplicit = true;
                entry.Pipeline = manager.Pipeline;
                int position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            return manager.Pipeline;
        }

        private ResolveDelegate<BuilderContext> PipelineFromUnregisteredTypeTransient(Type type, TransientLifetimeManager manager, int position)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                lock (manager)
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, this, Context.TypePipelineCache);
                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);
                        Debug.Assert(null != _registry);

                        // Replace pipeline in storage
                        lock (_syncLock)
                        {
                            _registry.Entries[position].Pipeline = pipeline;
                        }
                    }

                    return pipeline(ref context);
                }
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromUnregisteredTypePerResolve(Type type, PerResolveLifetimeManager manager)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                lock (manager)
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, this, Context.TypePipelineCache);
                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);

                        manager.PipelineDelegate = pipeline;
                        return pipeline(ref context);
                    }

                    var lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));
                    Debug.Assert(null != lifetime);

                    var value = lifetime.Get(LifetimeContainer);
                    if (LifetimeManager.NoValue != value) return value;

                    return pipeline(ref context);
                }
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromUnregisteredTypeDefault(Type type, LifetimeManager manager)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                lock (manager)
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, this, Context.TypePipelineCache);
                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);

                        manager.PipelineDelegate = pipeline;
                    }

                    var value = manager.Get(LifetimeContainer);
                    if (LifetimeManager.NoValue != value) return value;

                    return pipeline(ref context);
                }
            };
        }
    }
}
