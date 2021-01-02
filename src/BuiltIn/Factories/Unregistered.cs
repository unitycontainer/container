using Unity.Container;
using Unity.Extension;


namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        /// <summary>
        /// Default algorithm for unregistered type resolution
        /// </summary>
        public static object? UnregisteredAlgorithm(ref PipelineContext context)
        {
            var type = context.Type;
            var defaults = (Policies)context.Policies;

            // Get pipeline
            var pipeline = context.Policies.CompareExchange(type, defaults.ActivatePipeline, null);

            if (pipeline is null)
            {
                // Build and save pipeline
                pipeline = context.Container.Policies.FromTypeFactory(ref context);

                // TODO: Cache

                context.Policies.CompareExchange(type, pipeline, defaults.ActivatePipeline);
            }

            // Resolve
            context.Target = pipeline!(ref context);

            if (!context.IsFaulted) context.Registration?.SetValue(context.Target, context.Container.Scope);

            return context.Target;
        }
    }
}
