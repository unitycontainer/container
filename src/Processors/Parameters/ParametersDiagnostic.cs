using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
    {
        #region Diagnostic Parameter Factories

        protected virtual IEnumerable<Expression> CreateDiagnosticParameterExpressions(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);

                // Check if has default value

#if NET40
                Expression defaultValueExpr = null;
                if (parameter.DefaultValue is DBNull)
#else
                var defaultValueExpr = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : null;

                if (!parameter.HasDefaultValue)
#endif
                {
                    var ex = Expression.Variable(typeof(Exception));
                    var exData = Expression.MakeMemberAccess(ex, DataProperty);
                    var block = Expression.Block(parameter.ParameterType,
                        Expression.Call(exData, AddMethod,
                                Expression.Convert(NewGuid, typeof(object)),
                                Expression.Constant(parameter, typeof(object))),
                        Expression.Rethrow(parameter.ParameterType));

                    var tryBlock = Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context,
                                        BuilderContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);

                    yield return Expression.TryCatch(tryBlock, Expression.Catch(ex, block));
                }
                else
                {
                    var variable = Expression.Variable(parameter.ParameterType);
                    var resolve = Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context,
                                        BuilderContextExpression.ResolveParameterMethod,
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

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateDiagnosticParameterResolvers(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);

                // TODO: Add diagnostic for parameters

                // Check if has default value
#if NET40
                if (parameter.DefaultValue is DBNull)
#else
                if (!parameter.HasDefaultValue)
#endif
                {
                    // Plain vanilla case
                    yield return (ref BuilderContext context) =>
                    {
                        try
                        {
                            return context.Resolve(parameter, resolver);
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(Guid.NewGuid(), parameter);
                            throw;
                        }
                    };
                }
                else
                {
                    // Check if has default value
#if NET40
                    var defaultValue = !(parameter.DefaultValue is DBNull) ? parameter.DefaultValue : null;
#else
                    var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
#endif
                    yield return (ref BuilderContext context) =>
                    {
                        try
                        {
                            return context.Resolve(parameter, resolver);
                        }
                        catch
                        {
                            return defaultValue;
                        }
                    };
                }
            }
        }

        #endregion
    }
}
