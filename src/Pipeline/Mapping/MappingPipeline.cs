using Unity;
using Unity.Resolution;

namespace Unity
{
    public class MappingPipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder)
        {
            if (!builder.IsMapping) return builder.Pipeline();

            var requestedType = builder.Type;

            if (null != builder.Registration)
            {
                // Explicit Registration
                if (null == builder.Registration.Type) return builder.Pipeline();

                builder.Type = builder.Registration.Type;
            }
            else if (null != builder.TypeConverter)
            {
                builder.Type = builder.TypeConverter(builder.Type);
            }

            // If nothing to map or build required, just create it
            if (builder.BuildRequired || requestedType == builder.Type)
                return builder.Pipeline();

            var type = builder.Type;

            return builder.PipelineWithSeed((ref PipelineContext context) => context.Resolve(type));
        }

        #endregion
    }
}
