using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineFromRegistrationFactory(ref BuilderContext context)
        {
            switch (context.Registration?.Category)
            {
                case RegistrationCategory.Type:

                    // Check for Type Mapping
                    if (!context.Registration.RequireBuild && context.Contract.Type != context.Registration.Type)
                    {
                        var closure = new Contract(context.Registration.Type!, context.Contract.Name);

                        // Mapping resolver
                        return (ref BuilderContext c) =>
                        {
                            var contract = closure;
                            var map = c.Map(ref contract);
                            return map.Resolve();
                        };
                    }

                    return FromTypeFactory(ref context);

                case RegistrationCategory.Factory:
                    return ((Policies<BuilderContext>)context.Policies).FactoryPipeline;

                case RegistrationCategory.Instance:
                    return ((Policies<BuilderContext>)context.Policies).InstancePipeline;

                default:
                    return FromTypeFactory(ref context);
            }
        }

    }
}
