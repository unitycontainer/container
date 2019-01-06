using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;

namespace Unity.Processors
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        #region Parameter Factory

        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterInfo[] parameters, object[] injectors = null)
        {
            object[] resolvers = null != injectors && 0 == injectors.Length ? null : injectors;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);

                // Check if has default value
                var defaultValueExpr = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : null;

                if (!parameter.HasDefaultValue)
                {
                    // Plain vanilla case
                    yield return Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context, 
                                        BuilderContextExpression.ResolveParameter,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);
                }
                else
                {
                    var variable = Expression.Variable(parameter.ParameterType);
                    var resolve = Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context, 
                                        BuilderContextExpression.ResolveParameter,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);

                    yield return Expression.Block(new[] { variable }, new Expression[]
                    {
                        Expression.TryCatch(
                            Expression.Assign(variable, resolve),
                        Expression.Catch(typeof(Exception),
                            Expression.Assign(variable, defaultValueExpr))),
                        variable
                    });
                }
            }
        }

        #endregion
    }
}
