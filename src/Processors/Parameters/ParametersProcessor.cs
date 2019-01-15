using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Constructors

        protected ParametersProcessor(IPolicySet policySet, Type attribute)
            : base(policySet, attribute)
        {
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info) => info.DeclaringType;

        #endregion


        #region Expression 

        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
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
                                        BuilderContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);
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

        #endregion


        #region Resolution

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object injectors = null)
        {
            object[] resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var resolver = null == resolvers
                             ? FromAttribute(parameter)
                             : PreProcessResolver(parameter, resolvers[i]);

                if (!parameter.HasDefaultValue)
                {
                    // Plain vanilla case
                    yield return (ref BuilderContext context) => context.Resolve(parameter, resolver);
                }
                else
                {
                    // Check if has default value
                    var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

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


        #region Implementation

        private object PreProcessResolver(ParameterInfo parameter, object resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory<ParameterInfo> factory:
                    return factory.GetResolver<BuilderContext>(parameter);

                case Type type:
                    return typeof(Type) == parameter.ParameterType
                        ? type : FromAttribute(parameter);
            }

            return resolver;
        }

        private object FromAttribute(ParameterInfo info)
        {
            var defaultValue = info.HasDefaultValue ? info.DefaultValue : null;
            foreach (var node in AttributeFactories)
            {
                if (null == node.Factory) continue;
                var attribute = info.GetCustomAttribute(node.Type);
                if (null == attribute) continue;

                // If found match, use provided factory to create expression
                return node.Factory(attribute, info, defaultValue);
            }

            return info;
        }

        #endregion


        #region Attribute Factories

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            return (ref BuilderContext context) => context.Resolve(((ParameterInfo)info).ParameterType, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    return context.Resolve(((ParameterInfo)info).ParameterType, ((DependencyResolutionAttribute)attribute).Name);
                }
                catch
                {
                    return value;
                }
            };
        }

        #endregion
    }
}
