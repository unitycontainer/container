using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor 
    {
        #region Build Up

        protected virtual void SynchronizedLifetimeHandler(ref PipelineBuilder<object?> builder, SynchronizedLifetimeManager manager)
        {
            try
            {
                LifetimeManagerHandler(ref builder, manager);
            }
            catch
            {
                manager.Recover();
                throw;
            }
        }

        #endregion


        #region Resolution

        protected virtual void SynchronizedLifetimeHandler(ref PipelineBuilder<Pipeline?> builder, SynchronizedLifetimeManager manager)
        {
            // Build downstream pipeline
            LifetimeManagerHandler(ref builder, manager);
            
            var pipeline = builder.Target!;

            builder.Target = (ref ResolutionContext context) =>
            {
                try
                {
                    // Build downstream and save if success
                    pipeline.Invoke(ref context);
                }
                catch
                {
                    manager.Recover();
                    throw;
                }
            };
        }

        #endregion
    }
}
