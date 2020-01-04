using System.Collections.Generic;
using System.Linq.Expressions;

namespace Unity
{
    public partial class MappingPipeline : Pipeline
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            if (!builder.IsMapping) return builder.Express();

            var requestedType = builder.Type;

            if (null != builder.Registration)
            {
                // Explicit Registration
                if (null == builder.Registration.Type) return builder.Express();

                builder.Type = builder.Registration.Type;
            }
            else if (null != builder.TypeConverter)
            {
                builder.Type = builder.TypeConverter(builder.Type);
            }

            // If nothing to map or build required, just create it
            if (builder.BuildRequired || requestedType == builder.Type)
                return builder.Express();

            var type = builder.Type;

            return builder.Express((ref PipelineContext context) => context.Resolve(type));
        }

        #endregion
    }
}
