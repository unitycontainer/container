using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
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
            Type? generic = null;
            var resolver = builder.Registration.Get(typeof(ResolveDelegate<BuilderContext>)) ??
                           builder.ContainerContext.Get(builder.Type, typeof(ResolveDelegate<BuilderContext>));

            if (null == resolver)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                {
                    generic = builder.Type.GetGenericTypeDefinition();
                    resolver = builder.ContainerContext.Get(generic, builder.Registration.Name, typeof(ResolveDelegate<BuilderContext>)) ??
                               builder.ContainerContext.Get(generic, typeof(ResolveDelegate<BuilderContext>));
                }
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
                    factory = (TypeResolverFactory?)builder.ContainerContext.Get(builder.Type.GetGenericTypeDefinition(),
                                                                                 typeof(TypeResolverFactory));
                }
                else if (builder.Type.IsArray)
                {
                    if (builder.Type.GetArrayRank() == 1)
                    {
                        var resolve = ArrayResolver.Factory(builder.Type, builder.Registration);
                        return builder.Pipeline((ref BuilderContext context) => resolve(ref context));
                    }
                    else
                    {
                        var message = $"Invalid array {builder.Type}. Only arrays of rank 1 are supported";
                        return (ref BuilderContext context) => throw new InvalidOperationException(message,
                            new InvalidRegistrationException());
                    }
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
                    factory = (TypeResolverFactory?)builder.ContainerContext.Get(builder.Type.GetGenericTypeDefinition(),
                                                                                 typeof(TypeResolverFactory));
                }
                else if (builder.Type.IsArray)
                {
                    if (builder.Type.GetArrayRank() == 1)
                        return builder.Pipeline(ArrayResolver.Factory(builder.Type, builder.Registration));
                    else
                    {
                        var message = $"Invalid array {builder.Type}. Only arrays of rank 1 are supported";
                        return (ref BuilderContext context) => throw new InvalidOperationException(message,
                            new InvalidRegistrationException());
                    }
                }
            }

            return null != factory
                ? builder.Pipeline(factory(builder.Type, builder.Registration))
                : builder.Pipeline();
        }

        #endregion
    }
}
