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

            int position = 0;
            var collisions = 0;
            LifetimeManager manager;

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

                    // Has already been created
                    Debug.Assert(null != candidate.Pipeline);
                    return candidate.Pipeline;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create a Lifetime Manager
                manager = factory.LifetimeManager.CreateLifetimePolicy();
                manager.PipelineDelegate = manager switch
                {
                    TransientLifetimeManager  transient => PipelineFromOpenGenericTransient(type, factory, transient, _registry.Count),
                    SynchronizedLifetimeManager   synch => PipelineFromOpenGenericSynchronized(type, factory, synch),
                    PerResolveLifetimeManager peresolve => PipelineFromOpenGenericPerResolve(type, factory, peresolve),
                                                      _ => PipelineFromOpenGenericDefault(type, factory, manager)
                };

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Type = type;
                entry.Pipeline = manager.Pipeline;
                entry.Next = _registry.Buckets[targetBucket];
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            if (manager is IDisposable disposable) Context.Lifetime.Add(disposable);

            // Return pipeline
            return manager.Pipeline;
        }

        private ResolveDelegate<BuilderContext> PipelineFromOpenGenericTransient(Type type, ExplicitRegistration factory, TransientLifetimeManager manager, int position)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (manager)
                {
                    // Create if required
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, factory, manager, this);
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

        private ResolveDelegate<BuilderContext> PipelineFromOpenGenericSynchronized(Type type, ExplicitRegistration factory, SynchronizedLifetimeManager manager)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;
            Debug.Assert(null != manager);

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (manager)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, factory, manager, this);

                        pipeline = builder.Pipeline();
                        manager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);
                    }
                }

                try
                {
                    // Build withing the scope
                    return pipeline(ref context);
                }
                catch
                {
                    manager.Recover();
                    throw;
                }
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromOpenGenericPerResolve(Type type, ExplicitRegistration factory, PerResolveLifetimeManager manager)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                object value;
                LifetimeManager? lifetime;

                if (null != pipeline)
                {
                    if (null != (lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager))))
                    {
                        value = lifetime.Get(LifetimeContainer);
                        if (LifetimeManager.NoValue != value) return value;
                    }

                    return pipeline(ref context);
                }

                lock (manager)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, factory, manager, this);

                        pipeline = builder.Pipeline();
                        manager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);

                        value = pipeline(ref context);
                        context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));
                        return value;
                    }
                }

                if (null != (lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager))))
                {
                    value = lifetime.Get(LifetimeContainer);
                    if (LifetimeManager.NoValue != value) return value;
                }

                return pipeline(ref context);
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromOpenGenericDefault(Type type, ExplicitRegistration factory, LifetimeManager manager)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (manager)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, factory, manager, this);

                        pipeline = builder.Pipeline();
                        manager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);
                    }
                }

                return pipeline(ref context);
            };
        }
    }
}
