using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension<TContext>
    {
        public static ResolveDelegate<TContext> PipelineFromRegistrationFactory(ref TContext context)
        {
            switch (context.Registration?.Category)
            {
                case RegistrationCategory.Type:

                    // Check for Type Mapping
                    if (!context.Registration.RequireBuild && context.Contract.Type != context.Registration.Type)
                    {
                        var closure = new Contract(context.Registration.Type!, context.Contract.Name);

                        // Mapping resolver
                        return (ref TContext c) =>
                        {
                            var contract = closure;
                            var map = c.Map(ref contract);
                            return c.Container.Resolve(ref map);
                        };
                    }

                    return FromTypeFactory(ref context);

                case RegistrationCategory.Factory:
                    return ((Policies<TContext>)context.Policies).FactoryPipeline;

                case RegistrationCategory.Instance:
                    return ((Policies<TContext>)context.Policies).InstancePipeline;

                default:
                    return FromTypeFactory(ref context);
            }
        }

    }
}
