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
        private ResolveDelegate<BuilderContext> PipelineFromRegistration(ref HashKey key, ExplicitRegistration registration, int position)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            registration.LifetimeManager.PipelineDelegate = registration.LifetimeManager switch
            {
                TransientLifetimeManager    _ => PipelineFromRegistrationTransient(key.Type, registration, position),
                SynchronizedLifetimeManager _ => PipelineFromRegistrationSynchronized(key.Type, registration),
                PerResolveLifetimeManager   _ => PipelineFromRegistrationPerResolve(key.Type, registration),
                                            _ => PipelineFromRegistrationDefault(key.Type, registration)
            };

            return registration.LifetimeManager.Pipeline;
        }

        private ResolveDelegate<BuilderContext> PipelineFromRegistrationTransient(Type type, ExplicitRegistration registration, int position)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (registration)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, registration);

                        pipeline = builder.Pipeline();

                        Debug.Assert(null != pipeline);
                        Debug.Assert(null != _registry);

                        lock (_syncLock)
                        {
                            _registry.Entries[position].Pipeline = pipeline;
                        }
                    }

                    return pipeline(ref context);
                }
            };
        }

        private ResolveDelegate<BuilderContext> PipelineFromRegistrationSynchronized(Type type, ExplicitRegistration registration)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;
            var manager = registration.LifetimeManager as SynchronizedLifetimeManager;
            Debug.Assert(null != manager);

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (registration)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, registration);

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

        private ResolveDelegate<BuilderContext> PipelineFromRegistrationPerResolve(Type type, ExplicitRegistration registration)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;
            Debug.Assert(null != registration.LifetimeManager);

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

                lock (registration)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, registration);

                        pipeline = builder.Pipeline();
                        registration.LifetimeManager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);
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

        private ResolveDelegate<BuilderContext> PipelineFromRegistrationDefault(Type type, ExplicitRegistration registration)
        {
            ResolveDelegate<BuilderContext>? pipeline = null;
            Debug.Assert(null != registration.LifetimeManager);

            return (ref BuilderContext context) =>
            {
                if (null != pipeline) return pipeline(ref context);

                lock (registration)
                {
                    if (null == pipeline)
                    {
                        PipelineBuilder builder = new PipelineBuilder(type, registration);

                        pipeline = builder.Pipeline();
                        registration.LifetimeManager.PipelineDelegate = pipeline;

                        Debug.Assert(null != pipeline);
                    }
                }

                return pipeline(ref context);
            };
        }
    }
}
