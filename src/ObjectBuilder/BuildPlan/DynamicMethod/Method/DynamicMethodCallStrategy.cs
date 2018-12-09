using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod.Method
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to call
    /// chosen methods (as specified by the current <see cref="IMethodSelectorPolicy"/>)
    /// as part of object build up.
    /// </summary>
    public class DynamicMethodCallStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;
            var selector = context.Policies.GetPolicy<IMethodSelectorPolicy>(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name);

            foreach (var selection in selector.SelectMethods(ref context))
            {
                MethodInfo methodInfo;
                ParameterInfo[] parameters;
                object[] resolvers;

                switch (selection)
                {
                    case SelectedMethod selectedMethod:
                        methodInfo = selectedMethod.Method;
                        parameters = methodInfo.GetParameters();
                        resolvers = selectedMethod.GetResolvers();
                        break;

                    case IMethodBaseMember<MethodInfo> injectionMethod:
                        methodInfo = injectionMethod.GetInfo(context.Type);
                        parameters = methodInfo.GetParameters();
                        resolvers = injectionMethod.GetParameters();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                if (methodInfo.IsGenericMethodDefinition ||
                    parameters.Any(param => param.IsOut  || param.ParameterType.IsByRef))
                {
                    var format = methodInfo.IsGenericMethodDefinition
                        ? Constants.CannotInjectOpenGenericMethod
                        : Constants.CannotInjectMethodWithOutParam;

                    throw new IllegalInjectionMethodException(string.Format(CultureInfo.CurrentCulture,
                        format, methodInfo.DeclaringType.GetTypeInfo().Name, methodInfo.Name));
                }

                dynamicBuildContext.AddToBuildPlan(
                    Expression.Block(
                        Expression.Call(
                            Expression.Convert(
                                BuilderContextExpression<TBuilderContext>.Existing,
                                dynamicBuildContext.TypeToBuild),
                            methodInfo,
                            BuildMethodParameterExpressions<TBuilderContext>(parameters, resolvers))));

            }
        }

        #endregion


        #region Implementation

        private IEnumerable<Expression> BuildMethodParameterExpressions<TBuilderContext>(ParameterInfo[] parameters, object[] resolvers)
            where TBuilderContext : IBuilderContext
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                // Resolve all DependencyAttributes on this parameter, if any
                var attribute = parameter.GetCustomAttributes(false)
                    .OfType<DependencyResolutionAttribute>()
                    .FirstOrDefault();

                yield return 
                    BuilderContextExpression<TBuilderContext>.Resolve(parameter, attribute?.Name, resolvers[i]);
            }
        }

        #endregion
    }
}
