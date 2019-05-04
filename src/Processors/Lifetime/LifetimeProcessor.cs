using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class LifetimeProcessor : PipelineProcessor
    {
        public override IEnumerable<Expression> GetExpressions(Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? GetResolver(Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed)
        {
            var lifetime = registration.LifetimeManager;

            if (null == lifetime || lifetime is TransientLifetimeManager) return seed;

            // Per Resolve
            if (lifetime is PerResolveLifetimeManager)
                return (ref BuilderContext context) =>
                {
                    // Get it from context
                    var policy = (LifetimeManager?)context.Get(typeof(LifetimeManager));

                    // Return if holds value
                    var value = policy?.GetValue(context.ContainerContext.Lifetime) ?? LifetimeManager.NoValue;
                    if (LifetimeManager.NoValue != value) return value;

                    // Compose down the chain
                    value = seed?.Invoke(ref context);
                    policy?.SetValue(value, context.ContainerContext.Lifetime);

                    return value;
                };

            if (lifetime is SynchronizedLifetimeManager recovery)
                return (ref BuilderContext context) =>
                {
                    context.RequiresRecovery = recovery;

                    // Return if holds value
                    var value = lifetime.GetValue(context.ContainerContext.Lifetime);
                    if (LifetimeManager.NoValue != value) return value;

                    // Compose down the chain
                    value = seed?.Invoke(ref context);
                    lifetime.SetValue(value, context.ContainerContext.Lifetime);

                    return value;
                };

            return (ref BuilderContext context) =>
            {
                // Return if holds value
                var value = lifetime.GetValue(context.ContainerContext.Lifetime);
                if (LifetimeManager.NoValue != value) return value;

                // Compose down the chain
                value = seed?.Invoke(ref context);
                lifetime.SetValue(value, context.ContainerContext.Lifetime);

                return value;
            };
        }
    }
}
