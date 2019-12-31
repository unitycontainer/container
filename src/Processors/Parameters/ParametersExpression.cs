using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
    {


        #region Expression 

        protected virtual IEnumerable<Expression> CreateParameterExpressions(MethodBase info)
        {
            foreach (var parameter in info.GetParameters())
            { 
                yield return GetResolverExpression(parameter);
            }
        }


        protected virtual IEnumerable<Expression> CreateParameterExpressions(MethodBase info, object[] injectors)
        {
            var parameters = info.GetParameters();
            for(var i = 0; i < parameters.Length; i++)
            {
                yield return GetResolverExpression(parameters[i], injectors[i]);
            }
        }

        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterInfo[] parameters, object? injectors = null)
        {
            object[]? resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = PreProcessResolver(parameter, resolvers[i]);

                yield return Expression.Convert(
                                Expression.Call(BuilderContextExpression.Context,
                                    BuilderContextExpression.ResolveParameterMethod,
                                    Expression.Constant(parameter, typeof(ParameterInfo)),
                                    Expression.Constant(resolver, typeof(object))),
                                parameter.ParameterType);
            }
        }

        #endregion


        #region Expression 

        protected virtual Expression GetResolverExpression(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context,
                                BuilderContextExpression.ResolveParameterMethod,
                                Expression.Constant(info, typeof(ParameterInfo)),
                                Expression.Constant(attribute.Name, typeof(string)),
                                Expression.Constant(resolver, typeof(ResolveDelegate<BuilderContext>))),
                            info.ParameterType);
        }

        protected virtual Expression GetResolverExpression(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = data switch
            {
                IResolve policy                                   => policy.Resolve,
                IResolverFactory<ParameterInfo> parameterFactory  => parameterFactory.GetResolver<BuilderContext>(info),
                IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<BuilderContext>(info.ParameterType),
                Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<BuilderContext>(type),
                _                                                 => null
            };

            if (null == resolver)
            {
                return Expression.Convert(
                    Expression.Call(BuilderContextExpression.Context,
                                    BuilderContextExpression.OverrideParameterMethod,
                                    Expression.Constant(info, typeof(ParameterInfo)),
                                    Expression.Constant(attribute.Name, typeof(string)),
                                    Expression.Constant(data, typeof(object))),
                                info.ParameterType);
            }
            else
            {
                return Expression.Convert(
                    Expression.Call(BuilderContextExpression.Context,
                                    BuilderContextExpression.ResolveParameterMethod,
                                    Expression.Constant(info, typeof(ParameterInfo)),
                                    Expression.Constant(attribute.Name, typeof(string)),
                                    Expression.Constant(resolver, typeof(ResolveDelegate<BuilderContext>))),
                                info.ParameterType);
            }
        }

        #endregion
    }
}
