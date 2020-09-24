using Unity.Container;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor
    {
        #region PipelineBuilder

        public override object? Build(ref PipelineBuilder<object?> builder)
        {
            object? value;
            ref var context = ref builder.Context;

            switch (context.LifetimeManager)
            {
                case PerResolveLifetimeManager _:
                case TransientLifetimeManager _:
                    return builder.Build();

                case SynchronizedLifetimeManager sync:
                    
                    // Skip getting value if building up
                    if (null != context.Data) return builder.Build();

                    try
                    {
                        // Get value from the manager
                        value = sync.GetValue(context.LifetimeContainer);
                        if (!ReferenceEquals(RegistrationManager.NoValue, value))
                        {
                            context.Data = value;
                            return value;
                        }

                        // Build downstream and save if success
                        value = builder.Build();

                        sync.SetValue(value, context.LifetimeContainer);

                        return value;
                    }
                    catch
                    {
                        // Recover and rethrow
                        sync.Recover();
                        throw;
                    }


                case LifetimeManager manager:

                    // Skip getting value if building up
                    if (null != context.Data) return builder.Build();

                    // Get value from the manager
                    value = manager.GetValue(context.LifetimeContainer);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value))
                    {
                        context.Data = value;
                        return value;
                    }

                    // Build downstream and save if success
                    value = builder.Build();
                    manager.SetValue(value, context.LifetimeContainer);

                    return value;


                default:
                    return builder.Build();
            }
        }

        #endregion
    }
}
