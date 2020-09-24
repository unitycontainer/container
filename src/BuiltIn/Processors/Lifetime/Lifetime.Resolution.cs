using System;
using System.Diagnostics;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            ResolveDelegate<PipelineContext>? pipeline = builder.Build();
            Debug.Assert(null != pipeline);

            switch (builder.Context.LifetimeManager)
            {
                case PerResolveLifetimeManager _:
                case TransientLifetimeManager  _:
                    return pipeline;


                case SynchronizedLifetimeManager sync:
                    return (ref PipelineContext context) =>
                    {
                        // Skip getting value if building up
                        if (null != context.Data) return pipeline.Invoke(ref context);

                        try
                        {
                            // Get value from the manager
                            var value = sync.GetValue(context.LifetimeContainer);
                            if (!ReferenceEquals(RegistrationManager.NoValue, value))
                            {
                                context.Data = value;
                                return value;
                            }

                            // Build downstream and save if success
                            value = pipeline.Invoke(ref context);

                            sync.SetValue(value, context.LifetimeContainer);

                            return value;
                        }
                        catch
                        {
                            // Recover and rethrow
                            sync.Recover();
                            throw;
                        }
                    };


                case LifetimeManager manager:
                    return (ref PipelineContext context) =>
                    {
                        // Skip getting value if building up
                        if (null != context.Data) return pipeline.Invoke(ref context);

                        // Get value from the manager
                        var value = manager.GetValue(context.LifetimeContainer);
                        if (!ReferenceEquals(RegistrationManager.NoValue, value))
                        {
                            context.Data = value;
                            return value;
                        }

                        // Build downstream and save if success
                        value = pipeline.Invoke(ref context);

                        manager.SetValue(value, context.LifetimeContainer);
                        
                        return value;
                    };


                default:
                    return pipeline;
            }
        }

        #endregion
    }
}
