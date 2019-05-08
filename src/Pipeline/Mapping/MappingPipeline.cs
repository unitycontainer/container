using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public class MappingPipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
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

            return builder.Pipeline((ref BuilderContext context) => context.Resolve(type));
        }

        #endregion
    }
}
