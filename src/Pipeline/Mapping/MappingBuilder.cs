using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class MappingBuilder : PipelineBuilder
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                      Type type, ImplicitRegistration registration)
        {
            yield break;
        }

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

            return builder.Pipeline((ref BuilderContext context) => context.Resolve(type));
        }

        #endregion
    }
}
