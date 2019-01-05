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
                var resolver = null == resolvers ? parameter : PreProcessResolver(parameter, resolvers[i]);

                // Check if has default value
                var defaultValueExpr = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : null;

                // Check for registered attributes first
                var expression = FromAttribute(parameter, defaultValueExpr, resolver);
                if (null == expression)
                {
                    // Check if has default value
                    if (!parameter.HasDefaultValue)
                    {
                        // Plain vanilla case
                        expression = CallResolveExpression(parameter, null, resolver);
                    }
                    else
                    {
                        var variable = Expression.Variable(parameter.ParameterType);
                        var resolve = CallResolveExpression(parameter, null, resolver);

                        expression = Expression.Block(new[] { variable }, new Expression[]
                        {
                            Expression.TryCatch(
                                Expression.Assign(variable, resolve),
                            Expression.Catch(typeof(Exception),
                                Expression.Assign(variable, defaultValueExpr))),
                            variable
                        });

                    }
                }

                yield return expression;
            }

            Expression FromAttribute(ParameterInfo param, Expression member, object data)
            {
                foreach (var node in AttributeFactories)
                {
                    if (null == node.ExpressionFactory) continue;
                    var attribute = param.GetCustomAttribute(node.Type);
                    if (null == attribute) continue;

                    // If found match, use provided factory to create expression
                    return node.ExpressionFactory(attribute, member, param, param.ParameterType, data);
                }

                return null;
            }
        }

        #endregion


        #region Attribute Factory

        protected override Expression DependencyExpressionFactory(Attribute attribute, Expression member, object info, Type type, object resolver)
        {
            var parameter = (ParameterInfo)info;
            if (null == member)
            {
                // Plain vanilla case
                return CallResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver);
            }
            else
            {
                // Has default value
                var variable = Expression.Variable(parameter.ParameterType);

                return Expression.Block(new[] { variable }, new Expression[]
                {
                        Expression.TryCatch(
                            Expression.Assign(
                                variable,
                                CallResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver)),
                        Expression.Catch(typeof(Exception),
                            Expression.Assign(variable, member))),
                        variable
                });

            }
        }

        protected override Expression OptionalDependencyExpressionFactory(Attribute attribute, Expression member, object info, Type type, object resolver)
        {
            var parameter = (ParameterInfo)info;
            var variable = Expression.Variable(parameter.ParameterType);

            return Expression.Block(new[] { variable }, new Expression[]
            {
                Expression.TryCatch(
                    Expression.Assign(
                        variable,
                        CallResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver ?? OptionalDependencyAttribute.Instance)),
                Expression.Catch(typeof(Exception),
                    Expression.Assign(variable, member ?? Expression.Constant(null, parameter.ParameterType)))),
                variable
            });

        }

        #endregion


        #region Implementation

        private Expression CallResolveExpression(ParameterInfo parameter, string name, object resolver = null)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveParameter,
                    Expression.Constant(parameter, typeof(ParameterInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                parameter.ParameterType);
        }

        #endregion
    }
}
