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
            
            if (builder.Registration is ExplicitRegistration @explicit)
            {
                // Explicit Registration
                if (null == @explicit.Type) return builder.Pipeline();

                builder.Type = (null == @explicit.BuildType)
                    ? @explicit.Type
                    : @explicit.BuildType(@explicit.Type);
            }
            else if (builder.Registration is ImplicitRegistration @implicit)
            {
                // Implicit Registration
                if (null != @implicit.BuildType)
                    builder.Type = @implicit.BuildType(builder.Type);
            }

            // If nothing to map or build required, just create it
            if ((builder.Registration?.BuildRequired ?? false) || requestedType == builder.Type)
                return builder.Pipeline();

            var type = builder.Type;

            return builder.Pipeline((ref BuilderContext context) => context.Resolve(type));
        }

        #endregion
    }
}
