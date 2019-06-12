using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity
{
    public abstract partial class ParametersPipeline<TMemberInfo>
    {
        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterExpression[] expressions, ParameterInfo[] parameters, object? injectors)
        {
            object[]? resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterExpr = expressions[i];
                var parameterInfo = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameterInfo)
                             : PreProcessResolver(parameterInfo, resolvers[i]);

                // Check if has default value
#if NET40
                var defaultValueExpr = parameter.DefaultValue is DBNull
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : null;

                if (parameter.DefaultValue is DBNull)
#else
                var defaultValueExpr = parameterInfo.HasDefaultValue
                    ? Expression.Constant(parameterInfo.DefaultValue, parameterInfo.ParameterType)
                    : null;

                if (!parameterInfo.HasDefaultValue)
#endif
                {
                    // Plain vanilla case
                    yield return Expression.Assign(parameterExpr, Expression.Convert(
                                    Expression.Call(PipelineContextExpression.Context,
                                        PipelineContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameterInfo, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameterInfo.ParameterType));
                }
                else
                {
                    var resolve = Expression.Convert(
                                    Expression.Call(PipelineContextExpression.Context,
                                        PipelineContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameterInfo, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameterInfo.ParameterType);

                    yield return Expression.TryCatch(
                                    Expression.Assign(parameterExpr, resolve), 
                                 Expression.Catch(typeof(Exception), 
                                    Expression.Assign(parameterExpr, defaultValueExpr)));
                }
            }
        }
    }
}
