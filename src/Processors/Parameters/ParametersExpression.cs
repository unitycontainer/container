using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
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
            for(var i = 0; i < parameters.Length; i++)
            {
                yield return GetResolverExpression(parameters[i], injectors[i]);
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
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

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
