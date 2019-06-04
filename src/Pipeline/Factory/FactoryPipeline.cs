using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Factories;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public class FactoryPipeline : Pipeline
    {
        #region PipelineBuilder

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            // Skip if already have a resolver
            if (null != builder.Seed) return builder.Pipeline();

            // Try to get resolver
            Type? generic = null;
            var resolver = builder.Policies?.Get(typeof(ResolveDelegate<BuilderContext>)) ??
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
                    resolver = builder.ContainerContext.Get(generic, typeof(ResolveDelegate<BuilderContext>));
                }
            }

            // Process if found
            if (null != resolver) return builder.Pipeline((ResolveDelegate<BuilderContext>)resolver);
            
            // Try finding factory
            TypeFactoryDelegate? factory = builder.Policies?.Get<TypeFactoryDelegate>();

            if (builder.Policies is ExplicitRegistration @explicit)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                {
                    factory = (TypeFactoryDelegate?)builder.ContainerContext.Get(builder.Type.GetGenericTypeDefinition(),
                                                                                 typeof(TypeFactoryDelegate));
                }
                else if (null != builder.Type && builder.Type.IsArray)
                {
                    if (builder.Type.GetArrayRank() == 1)
                    {
                        var resolve = ArrayResolver.Factory(builder.Type, builder.ContainerContext.Container);
                        return builder.Pipeline((ref BuilderContext context) => resolve(ref context));
                    }
                    else
                    {
                        var message = $"Invalid array {builder.Type}. Only arrays of rank 1 are supported";
                        return (ref BuilderContext context) => throw new InvalidRegistrationException(message);
                    }
                }
            }
            else if(builder.Policies is ImplicitRegistration @implicit)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                {
                    factory = (TypeFactoryDelegate?)builder.ContainerContext.Get(builder.Type.GetGenericTypeDefinition(),
                                                                                 typeof(TypeFactoryDelegate));
                }
                else if (builder.Type?.IsArray ?? false)
                {
                    if (builder.Type?.GetArrayRank() == 1)
                        return builder.Pipeline(ArrayResolver.Factory(builder.Type, builder.ContainerContext.Container));
                    else
                    {
                        var message = $"Invalid array {builder.Type}. Only arrays of rank 1 are supported";
                        return (ref BuilderContext context) => throw new InvalidRegistrationException(message);
                    }
                }
            }

            Debug.Assert(null != builder.Type);

            return null != factory
                ? builder.Pipeline(factory(builder.Type, builder.ContainerContext.Container))
                : builder.Pipeline();
        }

        #endregion
    }
}
