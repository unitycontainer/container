using System;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public class MappingDiagnostic : MappingPipeline
    {

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            var requestedType = builder.Type;

            if (builder.Policies is ExplicitRegistration registration)
            {
                // Explicit Registration
                if (null == registration.Type) return builder.Pipeline();

                builder.Type = (null == registration.BuildType)
                    ? registration.Type
                    : registration.BuildType(registration.Type);
            }
            else
            {
                // Implicit Registration
                if (null != builder.BuildType)
                    builder.Type = builder.BuildType(builder.Type);
            }

            // If nothing to map or build required, just create it
            if (builder.BuildRequired || requestedType == builder.Type)
                return builder.Pipeline();

            var type = builder.Type;

            return builder.Pipeline((ref BuilderContext context) => 
            {
                try
                {
                    return context.Resolve(type);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), new Tuple<Type, Type>(requestedType, type));
                    throw;
                }
            });
        }

    }
}
