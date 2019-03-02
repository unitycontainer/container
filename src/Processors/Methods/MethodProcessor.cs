using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MethodProcessor : ParametersProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(IPolicySet policySet)
            : base(policySet, typeof(InjectionMethodAttribute))
        {
        }

        #endregion


        #region Selection

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            var info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredMethods)
                {
                    if (!member.IsFamily && !member.IsPrivate && !member.IsStatic)
                        yield return member;
                }

                info = info.BaseType?.GetTypeInfo();
            }
        }

        #endregion


        #region Overrides

        protected override void ValidateMember(MethodInfo info)
        {
            var parameters = info.GetParameters();
            if (info.IsGenericMethodDefinition || parameters.Any(param => param.IsOut || param.ParameterType.IsByRef))
            {
                var format = info.IsGenericMethodDefinition
                    ? "The method {1} on type {0} is marked for injection, but it is an open generic method. Injection cannot be performed."
                    : "The method {1} on type {0} has an out parameter. Injection cannot be performed.";

                throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture,
                    format, info.DeclaringType.GetTypeInfo().Name, info.Name));
            }
        }

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info, object resolvers)
        {
            return Expression.Call(
                Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                info, CreateParameterExpressions(info.GetParameters(), resolvers));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object resolvers)
        {
            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
            return (ref BuilderContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var parameters = new object[parameterResolvers.Length];
                for (var i = 0; i < parameters.Length; i++)
                    parameters[i] = parameterResolvers[i](ref c);

                info.Invoke(c.Existing, parameters);

                return c.Existing;
            };
        }

        #endregion
    }
}
