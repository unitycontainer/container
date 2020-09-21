using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor 
    {
        #region Build Up

        protected virtual void LifetimeManagerHandler(ref PipelineBuilder<object?> builder, LifetimeManager manager)
        {
            ref var context = ref builder.Context;

            // Skip getting value if building up
            if (null != context.Target)
            {
                builder.Build();
                return;
            }

            // Get value from the manager
            var existing = manager.GetValue(context.Scope);
            if (!ReferenceEquals(RegistrationManager.NoValue, existing))
            {
                context.FromValue(existing);
                return;
            }

            // Build downstream and save if success
            existing = builder.Build();

            if (!context.IsFaulted)
                manager.SetValue(existing, context.Scope);
        }

        #endregion


        #region Resolution

        protected virtual void LifetimeManagerHandler(ref PipelineBuilder<Pipeline?> builder, LifetimeManager manager)
        {
            // Build downstream pipeline
            var pipeline = builder.Build() ?? DefaultPipeline;

            builder.Target = (ref ResolutionContext context) =>
            {
                // Skip getting value if building up
                if (null != context.Target)
                {
                    pipeline.Invoke(ref context);
                    return;
                }

                // Get value from the manager
                var existing = manager.GetValue(context.Scope);
                if (!ReferenceEquals(RegistrationManager.NoValue, existing))
                {
                    context.FromValue(existing);
                    return;
                }

                // Build downstream and save if success
                pipeline.Invoke(ref context);

                if (!context.IsFaulted)
                    manager.SetValue(context.Target, context.Scope);
            };
        }

        #endregion
    }
}
