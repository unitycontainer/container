using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
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


        #region Overrides

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

        protected override Expression ValidateMemberInfo(MethodInfo info)
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

            return null;
        }

        protected override Expression CreateExpression(MethodInfo info, object[] resolvers, ParameterExpression variable) 
            => Expression.Call(variable, info, CreateParameterExpressions(info.GetParameters(), resolvers));

        #endregion
    }
}
