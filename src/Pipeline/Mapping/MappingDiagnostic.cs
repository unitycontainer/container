using System;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class MappingDiagnostic : MappingBuilder
    {

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            var requestedType = builder.Type;

            if (builder.Registration is ExplicitRegistration registration)
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
                if (null != builder.Registration.BuildType)
                    builder.Type = builder.Registration.BuildType(builder.Type);
            }

            // If nothing to map or build required, just create it
            if (builder.Registration.BuildRequired || requestedType == builder.Type)
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
