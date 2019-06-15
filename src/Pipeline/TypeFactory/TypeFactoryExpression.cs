using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Factories;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    public partial class TypeFactoryPipeline : Pipeline
    {
        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            // Skip if already have a resolver expression
            if (null != builder.SeedExpression) return builder.Express();

            // Try to get resolver
            Type? generic = null;
            var resolver = builder.Policies?.Get(typeof(ResolveDelegate<PipelineContext>)) ??
                           builder.ContainerContext.Get(builder.Type, typeof(ResolveDelegate<PipelineContext>));

            if (null == resolver)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != builder.Type && builder.Type.GetTypeInfo().IsGenericType)
#else
                if (null != builder.Type && builder.Type.IsGenericType)
#endif
                {
                    generic = builder.Type.GetGenericTypeDefinition();
                    resolver = builder.ContainerContext.Get(generic, typeof(ResolveDelegate<PipelineContext>));
                }
            }

            // Create an expression from resolver
            if (null != resolver) return builder.Express((ResolveDelegate<PipelineContext>)resolver);

            // Try finding factory
            TypeFactoryDelegate? factory = builder.Policies?.Get<TypeFactoryDelegate>();

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
                    return builder.Express((ref PipelineContext context) => resolve(ref context));
                }
                else
                {
                    var message = $"Invalid array {builder.Type}. Only arrays of rank 1 are supported";
                    return builder.Express((ref PipelineContext context) => throw new InvalidRegistrationException(message));
                }
            }

            Debug.Assert(null != builder.Type);

            return null != factory
                ? builder.Express(factory(builder.Type, builder.ContainerContext.Container))
                : builder.Express();
        }
    }
}
