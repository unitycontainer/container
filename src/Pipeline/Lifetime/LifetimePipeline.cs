using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    [SecuritySafeCritical]
    public class LifetimePipeline : Pipeline
    {
        #region PipelineBuilder


        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            var lifetime = builder.Registration.LifetimeManager;
            var pipeline = builder.Pipeline();
            var parent   = IntPtr.Zero;
            var type     = builder.Type;
            var registration = builder.Registration;


            // No Lifetime Manager
            if (null == lifetime || lifetime is TransientLifetimeManager)
            {
                return (ref BuilderContext context) => 
                {
                    // In Sync mode just execute pipeline
                    if (!context.Async) return pipeline(ref context);

                    // Async mode
                    var list = context.List;
                    var unity = context.ContainerContext;
                    var overrides = context.Overrides;
#if !NET40
                    unsafe
                    {
                        var thisContext = this;
                        parent = new IntPtr(Unsafe.AsPointer(ref thisContext));
                    }
#endif
                    // Create and return a task that creates an object
                    return Task.Factory.StartNew(() => 
                    {
                        var c = new BuilderContext
                        {
                            List             = list,
                            Type             = type,
                            ContainerContext = unity,
                            Registration     = registration,
                            Overrides        = overrides,
                            DeclaringType    = type,
                            Parent           = parent,
                        };

                        // Execute pipeline
                        return pipeline(ref c);
                    });
                };
            }


            // Per Resolve Lifetime Manager
            if (lifetime is PerResolveLifetimeManager)
                return (ref BuilderContext context) =>
                {
                    object value;

                    // Get it from context
                    var policy = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    if (null != policy)
                    {
                        value = policy.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value) return value;
                    }

                    // Compose down the chain
                    value = pipeline(ref context);
                    policy?.SetValue(value, context.ContainerContext.Lifetime);

                    return value;
                };


            // Requires Recovery during resolution
            if (lifetime is SynchronizedLifetimeManager recoverableManager)
            {
                return (ref BuilderContext context) =>
                {
                    try
                    {
                        // Return if holds value
                        var value = lifetime.GetValue(context.ContainerContext.Lifetime);
                        if (LifetimeManager.NoValue != value) return value;

                        // Compose 
                        value = pipeline(ref context);

                        // Set value
                        lifetime.SetValue(value, context.ContainerContext.Lifetime);
                        return value;
                    }
                    catch when (null != recoverableManager)
                    {
                        recoverableManager.Recover();
                        throw;
                    }
                };
            }

            return (ref BuilderContext context) =>
            {
                // Return if holds value
                var value = lifetime.GetValue(context.ContainerContext.Lifetime);
                if (LifetimeManager.NoValue != value) return value;

                // Compose down the chain
                value = pipeline(ref context);
                lifetime.SetValue(value, context.ContainerContext.Lifetime);

                return value;
            };
        }

        #endregion
    }
}
