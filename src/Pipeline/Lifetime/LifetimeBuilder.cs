using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class LifetimeBuilder : PipelineBuilder
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            var lifetime = builder.Registration.LifetimeManager;
            var pipeline = builder.Pipeline();

            if (null == lifetime || lifetime is TransientLifetimeManager) return pipeline;

            // The last builder in the Pipeline, just get the value
            if (null == pipeline)
                return (ref BuilderContext context) =>
                {
                    var value = lifetime.GetValue(context.Lifetime);
                    return LifetimeManager.NoValue != value ? value : null;
                };

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
                        value = policy.GetValue(context.Lifetime);
                        if (LifetimeManager.NoValue != value) return value;
                    }

                    // Compose down the chain
                    value = pipeline(ref context);
                    policy?.SetValue(value, context.Lifetime);

                    return value;
                };

            // Requires Recovery during resolution
            if (lifetime is SynchronizedLifetimeManager recovery)
                return (ref BuilderContext context) =>
                {
                    context.RequiresRecovery = recovery;

                    // Return if holds value
                    var value = lifetime.GetValue(context.Lifetime);
                    if (LifetimeManager.NoValue != value) return value;

                    // Compose down the chain
                    value = pipeline(ref context);
                    lifetime.SetValue(value, context.Lifetime);

                    return value;
                };

            return (ref BuilderContext context) =>
            {
                // Return if holds value
                var value = lifetime.GetValue(context.Lifetime);
                if (LifetimeManager.NoValue != value) return value;

                // Compose down the chain
                value = pipeline(ref context);
                lifetime.SetValue(value, context.Lifetime);

                return value;
            };
        }

        #endregion
    }
}
