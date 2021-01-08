using System;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        #region Fields

        private static PipelineFactory<BuilderContext> TypeFactory = FromTypeFactory;

        #endregion


        public static ResolveDelegate<BuilderContext> PipelineFromRegistrationFactory(ref BuilderContext context)
        {
            switch (context.Registration?.Category)
            {
                case RegistrationCategory.Type:

                    // Check for Type Mapping
                    if (!context.Registration.RequireBuild && context.Contract.Type != context.Registration.Type)
                    {
                        var closure = new Contract(context.Registration.Type!, context.Contract.Name);
                        return (ref BuilderContext c) =>
                        {
                            // Mapping resolver
                            var contract = closure;
                            return c.MapTo(ref contract);
                        };
                    }

                    return TypeFactory(ref context);

                case RegistrationCategory.Factory:
                    return ((Policies<BuilderContext>)context.Policies).FactoryPipeline;

                case RegistrationCategory.Instance:
                    return ((Policies<BuilderContext>)context.Policies).InstancePipeline;

                default:
                    return TypeFactory(ref context);
            }
        }


        #region Implementation

        private static void OnTypeFactoryChanged(Type? target, Type type, object? policy)
            => TypeFactory = (PipelineFactory<BuilderContext>)(policy
                ?? throw new ArgumentNullException(nameof(policy), "Factory must be valid delegate"));

        #endregion
    }
}
