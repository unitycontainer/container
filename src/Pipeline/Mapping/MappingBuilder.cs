using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            var request = builder.Type;

            // Implicit Registration
            if (builder.Registration is ExplicitRegistration explicitRegistration)
            {
                builder.Type = explicitRegistration.Type ?? builder.Type;
            }
            else
            {
                if (null != builder.Registration.Map) builder.Type = builder.Registration.Map(builder.Type);
            }

            if (builder.Registration.BuildRequired || request == builder.Type)
                return builder.Pipeline();

            var type = builder.Type;

            if (builder.Container.IsRegistered(type))
                return (ref BuilderContext context) => context.Resolve(type, context.Name);

            var pipeline = builder.Pipeline();
            return (ref BuilderContext context) =>
            {
                if (context.Container.IsRegistered(context.Type, context.Name))
                    return context.Resolve(type, context.Name);
                else
                    return pipeline?.Invoke(ref context);
            };
        }

        #endregion
    }
}
