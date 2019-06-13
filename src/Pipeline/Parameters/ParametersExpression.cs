using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity
{
    public partial class ParametersProcessor
    {
        public Expression ParameterExpression(ParameterExpression expression, ParameterInfo parameter) 
            => ParameterExpressionFactory(expression, parameter, FromAttribute(parameter));

        public virtual Expression ParameterExpression(ParameterExpression expression, ParameterInfo parameter, object injector) 
            => ParameterExpressionFactory(expression, parameter, PreProcessResolver(parameter, injector));

        protected virtual Expression ParameterExpressionFactory(ParameterExpression expression, ParameterInfo parameter, object resolver)
        {
            // Check if has default value
#if NET40
            var defaultValueExpr = parameter.DefaultValue is DBNull
                ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                : null;

            if (parameter.DefaultValue is DBNull)
#else
            var defaultValueExpr = parameter.HasDefaultValue
                ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                : null;

            if (!parameter.HasDefaultValue)
#endif
            {
                // Plain vanilla case
                return Expression.Assign(expression, Expression.Convert(
                                Expression.Call(PipelineContextExpression.Context,
                                    PipelineContextExpression.ResolveParameterMethod,
                                    Expression.Constant(parameter, typeof(ParameterInfo)),
                                    Expression.Constant(resolver, typeof(object))),
                                parameter.ParameterType));
            }
            else
            {
                var resolve = Expression.Convert(
                                Expression.Call(PipelineContextExpression.Context,
                                    PipelineContextExpression.ResolveParameterMethod,
                                    Expression.Constant(parameter, typeof(ParameterInfo)),
                                    Expression.Constant(resolver, typeof(object))),
                                parameter.ParameterType);

                return Expression.TryCatch(
                                Expression.Assign(expression, resolve),
                             Expression.Catch(typeof(Exception),
                                Expression.Assign(expression, defaultValueExpr)));
            }
        }
    }
}
