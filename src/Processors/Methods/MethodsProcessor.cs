using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Processors
{
    public class MethodsProcessor : MethodBaseInfoProcessor<MethodInfo>
    {
        #region Constructors

        public MethodsProcessor()
            : base(new (Type type, Converter<MethodInfo, object> factory)[]
                { (typeof(InjectionMethodAttribute), info => info) })
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

        #endregion


        #region BuilderStrategy

        /// <inheritdoc />
        public override IEnumerable<Expression> GetEnumerator(ref BuilderContext context)
        {
            var selector = GetPolicy<ISelect<MethodInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var methods = selector.Select(ref context);

            return GetEnumerator(context.Type, context.Name, context.Variable, methods);
        }

        #endregion


        #region Implementation

        private IEnumerable<Expression> GetEnumerator(Type type, string name, ParameterExpression variable, IEnumerable<object> methods)
        {
            foreach (var method in methods)
            {
                switch (method)
                {
                    case MethodInfo methodInfo:
                        VerifyMethodInfo(methodInfo);
                        yield return Expression.Call(variable, methodInfo,
                            BuildMethodParameterExpressions(methodInfo.GetParameters(), null));
                        break;

                    case MethodBaseMember<MethodInfo> injectionField:
                        var (info, resolvers) = injectionField.FromType(type);
                        VerifyMethodInfo(info);
                        yield return Expression.Call(variable, info,
                            BuildMethodParameterExpressions(info.GetParameters(), resolvers));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of field");
                }
            }
        }

        private void VerifyMethodInfo(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (methodInfo.IsGenericMethodDefinition || parameters.Any(param => param.IsOut || param.ParameterType.IsByRef))
            {
                var format = methodInfo.IsGenericMethodDefinition
                    ? Constants.CannotInjectOpenGenericMethod
                    : Constants.CannotInjectMethodWithOutParam;

                throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture,
                    format, methodInfo.DeclaringType.GetTypeInfo().Name, methodInfo.Name));
            }

        }

        private IEnumerable<Expression> BuildMethodParameterExpressions(ParameterInfo[] parameters, object[] resolvers)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                yield return
                    BuilderContextExpression.Resolve(parameter, attribute?.Name, resolvers?[i]);
            }
        }

        #endregion

    }
}
