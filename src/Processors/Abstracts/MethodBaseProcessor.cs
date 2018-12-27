using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;

namespace Unity.Processors
{
    public abstract class MethodBaseInfoProcessor<TMemberInfo> : MemberBuildProcessor<TMemberInfo, object[]>
                                             where TMemberInfo : MethodBase
    {
        #region Fields

        private static readonly MethodInfo ResolveParameter =
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

        public MethodBaseInfoProcessor(Type attribute)
            : base(new (Type type, MemberExpressionFactory factory)[] { (attribute, null) })
        {
            // TODO: Optimize 
            Add(typeof(DependencyAttribute),         DependencyParameterExpressionFactory);
            Add(typeof(OptionalDependencyAttribute), OptionalDependencyParameterExpressionFactory);
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info) => info.DeclaringType;

        protected override IEnumerable<Expression> GetEnumerator(Type type, string name, IEnumerable<object> members)
        {
            foreach (var member in members)
            {
                switch (member)
                {
                    case TMemberInfo memberInfo:
                        yield return ValidateMemberInfo(memberInfo) ??
                                     CreateExpression(memberInfo, null);
                        break;

                    case MethodBaseMember<TMemberInfo> injectionMember:
                        var (info, resolvers) = injectionMember.FromType(type);
                        yield return ValidateMemberInfo(info) ??
                                     CreateExpression(info, resolvers);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MethodBase<{typeof(TMemberInfo)}> type");
                }
            }
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
                foreach (var pair in ResolverFactories)
                {
                    if (null == pair.factory) continue;
                    var attribute = param.GetCustomAttribute(pair.type);
                    if (null == attribute) continue;

                    // If found match, use provided factory to create expression
                    return pair.factory(attribute, member, param, param.ParameterType, null, data);
                }

                return null;
            }
        }

        // Default ParameterInfo expression factory for [Dependency] attribute
        Expression DependencyParameterExpressionFactory(Attribute attribute, Expression member, object info, Type type, string name, object resolver)
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

        // Default ParameterInfo expression factory for [OptionalDependency] attribute
        Expression OptionalDependencyParameterExpressionFactory(Attribute attribute, Expression member, object info, Type type, string name, object resolver)
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


        #region Implementation

        private Expression ResolveExpression(ParameterInfo parameter, string name, object resolver = null)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveParameter,
                    Expression.Constant(parameter, typeof(ParameterInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                parameter.ParameterType);
        }

        protected abstract Expression CreateExpression(TMemberInfo info, object[] resolvers);

        protected virtual Expression ValidateMemberInfo(TMemberInfo info) => null;

        #endregion
    }
}
