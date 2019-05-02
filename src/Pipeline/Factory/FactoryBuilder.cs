using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Factories;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class FactoryBuilder : PipelineBuilder
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                      Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineContext builder)
        {
            // Try to get resolver
            var resolver = builder.Registration.Get(typeof(ResolveDelegate<BuilderContext>)) ??
                           builder.Container.GetPolicy(builder.Type, typeof(ResolveDelegate<BuilderContext>));

            if (null == resolver && builder.Registration is ExplicitRegistration explicitRegistration)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                    resolver = builder.Container.GetPolicy(builder.Type.GetGenericTypeDefinition(), typeof(ResolveDelegate<BuilderContext>));
            }

            // Process if found
            if (null != resolver) return builder.Pipeline((ResolveDelegate<BuilderContext>)resolver);


            // Try finding factory
            TypeResolverFactory? factory = builder.Registration.Get<TypeResolverFactory>();

            if (builder.Registration is ExplicitRegistration registration)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                {
                    factory = (TypeResolverFactory?)builder.Container.GetPolicy(builder.Type.GetGenericTypeDefinition(),
                                                                                   typeof(TypeResolverFactory));
                }
                else if (builder.Type.IsArray)
                {
                    var resolve = ArrayResolver.Factory(builder.Type, builder.Registration);
                    return builder.Pipeline((ref BuilderContext context) => resolve(ref context));
                }
            }
            else
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (builder.Type.GetTypeInfo().IsGenericType)
#else
                if (builder.Type.IsGenericType)
#endif
                {
                    factory = (TypeResolverFactory?)builder.Container.GetPolicy(builder.Type.GetGenericTypeDefinition(),
                                                                                   typeof(TypeResolverFactory));
                }
                else if (builder.Type.IsArray)
                {
                    return builder.Pipeline(ArrayResolver.Factory(builder.Type, builder.Registration));
                }
            }

            return null != factory
                ? builder.Pipeline(factory(builder.Type, builder.Registration))
                : builder.Pipeline();
        }

        #endregion
    }
}
