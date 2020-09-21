using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor 
    {

        #region Build Up

        protected virtual void PerResolveLifetimeHandler(ref PipelineBuilder<object?> builder, PerResolveLifetimeManager manager)
        {
            builder.Context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(builder.Build()));
        }

        #endregion


        #region Resolution

        protected virtual void PerResolveLifetimeHandler(ref PipelineBuilder<Pipeline?> builder, PerResolveLifetimeManager manager)
        {
            // Build downstream pipeline
            LifetimeManagerHandler(ref builder, manager);
            
            var pipeline = builder.Target!;

            builder.Target =(ref ResolutionContext context) =>
            {
                // Execute Pipeline
                pipeline.Invoke(ref context);

                // Save resolved value in per resolve singleton
                context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(context.Target));
            };
        }

        #endregion
    }
}
