using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract class MethodBaseInfoProcessor<TMemberInfo> : BuildMemberProcessor<TMemberInfo, object[]>
                                             where TMemberInfo : MethodBase
    {
        #region Fields

        private readonly MethodInfo ResolveParameter =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(ParameterInfo) == parameters[0].ParameterType;
                });

        #endregion

        
        #region Constructors

        public MethodBaseInfoProcessor(IPolicySet policySet, Type attribute)
            : base(policySet)
        {
            Add(attribute, (MemberResolverFactory)null);
            Add(attribute, (MemberExpressionFactory)null);
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info) => info.DeclaringType;

        protected override IEnumerable<object> SelectMembers(Type type, IEnumerable<TMemberInfo> members, InjectionMember[] injectors)
        {
            // Select Injected Members
            if (null != injectors)
            {
                foreach (var injectionMember in injectors)
                {
                    if (injectionMember is InjectionMember<TMemberInfo, object[]>)
                        yield return injectionMember;
                }
            }

            if (null == members) yield break;

            // Select Attributed members
            foreach (var member in members)
            {
                foreach (var pair in ResolverFactories)
                {
                    if (!member.IsDefined(pair.type)) continue;

                    yield return member;
                    break;
                }
            }
        }

        #endregion


        #region Parameter Resolution Factories

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object[] injectors)
        {
            object[] resolvers = null != injectors && 0 == injectors.Length ? null : injectors;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = resolvers?[i];

                // Check if has default value
                var defaultValue = parameter.HasDefaultValue
                    ? parameter.DefaultValue
                    : null;

                // Check for registered attributes first
                var expression = FromAttribute(parameter, defaultValue, resolver);
                if (null == expression)
                {
                    // Check if has default value
                    if (null == defaultValue)
                    {
                        // Plain vanilla case
                        expression = (ref BuilderContext context) => context.Resolve(parameter, null, resolver);
                    }
                    else
                    {
                        expression = (ref BuilderContext context) => 
                        {
                            try
                            {
                                return context.Resolve(parameter, null, resolver);
                            }
                            catch
                            {
                                return parameter.HasDefaultValue
                                ? parameter.DefaultValue
                                : null;
                            }
                        };
                    }
                }

                yield return expression;
            }

            ResolveDelegate<BuilderContext> FromAttribute(ParameterInfo param, object defaultValue, object data)
            {
                foreach (var pair in ResolverFactories)
                {
                    if (null == pair.factory) continue;
                    var attribute = param.GetCustomAttribute(pair.type);
                    if (null == attribute) continue;

                    // If found match, use provided factory to create expression
                    return pair.factory(attribute, param, data, defaultValue);
                }

                return null;
            }
        }

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            return (ref BuilderContext context) => 
            {
                if (null == defaultValue)
                    return context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver);
                else
                {
                    try
                    {
                        return context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver);
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            };
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    return context.Resolve((ParameterInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver);
                }
                catch
                {
                    return defaultValue;
                }
            };
        }

        #endregion


        #region Parameter Expression Factories

        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterInfo[] parameters, object[] injectors)
        {
            object[] resolvers = null != injectors && 0 == injectors.Length ? null : injectors;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = resolvers?[i];

                // Check if has default value
                var defaultValueExpr = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : null;

                // Check for registered attributes first
                var expression = FromAttribute(parameter, defaultValueExpr, resolver);
                if (null == expression)
                {
                    // Check if has default value
                    if (null == defaultValueExpr)
                    {
                        // Plain vanilla case
                        expression = ResolveExpression(parameter, null, resolver);
                    }
                    else
                    {
                        var variable = Expression.Variable(parameter.ParameterType);
                        var resolve = ResolveExpression(parameter, null, resolver);

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
                foreach (var pair in ExpressionFactories)
                {
                    if (null == pair.factory) continue;
                    var attribute = param.GetCustomAttribute(pair.type);
                    if (null == attribute) continue;

                    // If found match, use provided factory to create expression
                    return pair.factory(attribute, member, param, param.ParameterType, data);
                }

                return null;
            }
        }

        protected override Expression DependencyExpressionFactory(Attribute attribute, Expression member, object info, Type type, object resolver)
        {
            var parameter = (ParameterInfo)info;
            if (null == member)
            {
                // Plain vanilla case
                return ResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver);
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
                                ResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver)),
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
                        ResolveExpression(parameter, ((DependencyResolutionAttribute)attribute).Name, resolver)),
                Expression.Catch(typeof(Exception),
                    Expression.Assign(variable, member ?? Expression.Constant(null, parameter.ParameterType)))),
                variable
            });

        }

        #endregion


        #region Expression Implementation

        private Expression ResolveExpression(ParameterInfo parameter, string name, object resolver = null)
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
