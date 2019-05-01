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

        public override ResolveDelegate<BuilderContext>? Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                               Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed)
        {
            var request = type;

            // Implicit Registration
            if (registration is ExplicitRegistration explicitRegistration)
            {
                type = explicitRegistration.Type ?? type;
            }
            else
            {
                if (null != registration.Map) type = registration.Map(type);
            }

            if (registration.BuildRequired || request == type)
                return Pipeline(container, enumerator, type, registration, seed);

            if (container.IsRegistered(type))
                return (ref BuilderContext context) => context.Resolve(type, context.Name);

            var pipeline = Pipeline(container, enumerator, type, registration, seed);
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
