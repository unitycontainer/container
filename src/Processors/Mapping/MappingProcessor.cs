using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MappingProcessor : PipelineProcessor
    {
        public override IEnumerable<Expression> GetExpressions(Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? GetResolver(Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed)
        {
            if (registration is ExplicitRegistration containerRegistration)
            {
                if (null == containerRegistration.Type || type == containerRegistration.Type) return seed;
            }

            //if ( null == registration.Map)
            //    return (ref BuilderContext context) =>
            //    {
            //        if (!context.Registration.BuildRequired && ((UnityContainer)context.Container).IsRegistered(ref context) && null != context.Type)
            //        {
            //            return context.Resolve();
            //        }

            //        return seed?.Invoke(ref context);
            //    };


            return (ref BuilderContext context) =>
            {
                var map = context.Registration.Map;
                if (null != map && null != context.Type) context.Type = map(context.Type);

                if (!(context.Registration).BuildRequired &&
                    ((UnityContainer)context.Container).IsRegistered(ref context) &&
                    null != context.Type)
                {
                    return context.Resolve();
                }

                return seed?.Invoke(ref context);
            };
        }
    }
}
