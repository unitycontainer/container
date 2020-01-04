using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class MethodBasePipeline<TMemberInfo>
    {
        #region Expression

        protected virtual IEnumerable<Expression> ParameterExpressions(MethodBase info)
        {
            foreach (var parameter in info.GetParameters())
            {
                yield return GetResolverExpression(parameter);
            }
        }


        protected virtual IEnumerable<Expression> ParameterExpressions(MethodBase info, object[] injectors)
        {
            var parameters = info.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                yield return GetResolverExpression(parameters[i], injectors[i]);
            }
        }

        protected virtual Expression GetResolverExpression(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return Expression.Convert(
                Expression.Call(PipelineContextExpression.Context,
                                PipelineContextExpression.ResolveParameterMethod,
                                Expression.Constant(info, typeof(ParameterInfo)),
                                Expression.Constant(attribute.Name, typeof(string)),
                                Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                            info.ParameterType);
        }

        protected virtual Expression GetResolverExpression(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return Expression.Convert(
                    Expression.Call(PipelineContextExpression.Context,
                                    PipelineContextExpression.OverrideParameterMethod,
                                    Expression.Constant(info, typeof(ParameterInfo)),
                                    Expression.Constant(attribute.Name, typeof(string)),
                                    Expression.Constant(data, typeof(object))),
                                info.ParameterType);
            }
            else
            {
                return Expression.Convert(
                    Expression.Call(PipelineContextExpression.Context,
                                    PipelineContextExpression.ResolveParameterMethod,
                                    Expression.Constant(info, typeof(ParameterInfo)),
                                    Expression.Constant(attribute.Name, typeof(string)),
                                    Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                                info.ParameterType);
            }
        }

        #endregion


        #region Resolution

        protected virtual ResolveDelegate<PipelineContext>[] ParameterResolvers(MethodBase info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            var resolvers = new ResolveDelegate<PipelineContext>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            { 
                resolvers[i] = GetResolverDelegate(parameters[i]);
            }

            return resolvers;
        }

        protected virtual ResolveDelegate<PipelineContext>[] ParameterResolvers(MethodBase info, object[] data)
        {
            ParameterInfo[] parameters = info.GetParameters();
            var resolvers = new ResolveDelegate<PipelineContext>[parameters.Length];
            
            Debug.Assert(parameters.Length == data.Length);

            for (var i = 0; i < parameters.Length; i++)
            { 
                resolvers[i] = GetResolverDelegate(parameters[i], data[i]);
            }

            return resolvers;
        }

        protected virtual ResolveDelegate<PipelineContext> GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        protected virtual ResolveDelegate<PipelineContext> GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
                return (ref PipelineContext context) => context.Override(info, attribute.Name, data);
            else
                return (ref PipelineContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data) 
            => data switch
        {
            IResolve policy                                   => policy.Resolve,
            IResolverFactory<ParameterInfo> fieldFactory      => fieldFactory.GetResolver<PipelineContext>(info),
            IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<PipelineContext>(info.ParameterType),
            Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<PipelineContext>(type),
            _                                                 => null
        };

        #endregion
    }
}
