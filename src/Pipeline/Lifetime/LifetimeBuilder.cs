using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    [SecuritySafeCritical]
    public class LifetimeBuilder : PipelineBuilder
    {
        private readonly Task<object?> _nullTask = Task.FromResult<object?>(null);

        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            var lifetime = builder.Registration.LifetimeManager;
            var pipeline = builder.Pipeline();

            //var resolver = lifetime switch
            //{
            //    PerResolveLifetimeManager   _ => ((ref BuilderContext context) => context.Existing),
            //    SynchronizedLifetimeManager _ => ((ref BuilderContext context) => context.Existing),
            //    _                             => (ResolveDelegate<BuilderContext>)((ref BuilderContext context) => context.Existing)
            //};

            var parent       = IntPtr.Zero;
            var type         = builder.Type;
            var plan         = builder.ContainerContext.Container.BuilderContextPipeline;
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
                            ResolvePipeline  = plan,
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

                        // Compose down the chain
                        value = pipeline(ref context);
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
