using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Processors
{
    public class MethodsProcessor : MethodBaseInfoProcessor<MethodInfo>
    {
        #region Constructors

        public MethodsProcessor()
            : base(typeof(InjectionMethodAttribute))
        {
        }

        #endregion


        #region Selection

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        protected override MethodInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetMethodsHierarchical()
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
#endif
        }

        #endregion


        #region Building Expression

        protected override Expression BuildMemberExpression(MethodInfo info, string name, object[] resolvers)
        {
            ValidateMethod(info);

            return Expression.Call(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                           info, CreateParameterExpressions(info.GetParameters(), resolvers));
        }

        #endregion


        #region Building Resolver

        protected override ResolveDelegate<BuilderContext> BuildMemberResolver(MethodInfo info, string name, object[] resolvers)
        {
            ValidateMethod(info);

            var parameterResolvers = CreateParameterResolvers(info.GetParameters(), resolvers).ToArray();
            return (ref BuilderContext c) =>
            {
                if (null != c.Existing)
                {
                    var parameters = new object[parameterResolvers.Length];
                    for (var i = 0; i < parameters.Length; i++)
                        parameters[i] = parameterResolvers[i](ref c);

                    info.Invoke(c.Existing, parameters);
                }

                return c.Existing;
            };
        }

        #endregion


        #region Implementation

        private void ValidateMethod(MethodInfo info)
        {
            var parameters = info.GetParameters();
            if (info.IsGenericMethodDefinition || parameters.Any(param => param.IsOut || param.ParameterType.IsByRef))
            {
                var format = info.IsGenericMethodDefinition
                    ? Constants.CannotInjectOpenGenericMethod
                    : Constants.CannotInjectMethodWithOutParam;

                throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture,
                    format, info.DeclaringType.GetTypeInfo().Name, info.Name));
            }

        }

        #endregion
    }
}
