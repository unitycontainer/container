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

            if (builder.Policies is ExplicitRegistration @explicit)
            {
                // Explicit Registration
                if (null == @explicit.Type) return builder.Pipeline();

                builder.Type = (null == @explicit.BuildType)
                    ? @explicit.Type
                    : @explicit.BuildType(@explicit.Type);
            }
            else if (null != builder.TypeConverter)
            {
                builder.Type = builder.TypeConverter(builder.Type);
            }

            // If nothing to map or build required, just create it
            if (builder.BuildRequired || requestedType == builder.Type)
                return builder.Pipeline();

            var type = builder.Type;

            return builder.Pipeline((ref BuilderContext context) => context.Resolve(type));
        }

        #endregion
    }
}
