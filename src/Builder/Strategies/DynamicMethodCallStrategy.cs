using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder.Expressions;
using Unity.Exceptions;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Strategies;

namespace Unity.Builder.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to call
    /// chosen methods (as specified by the current <see cref="IMethodSelectorPolicy"/>)
    /// as part of object build up.
    /// </summary>
    public class DynamicMethodCallStrategy : BuilderStrategy// CompiledStrategy<MethodInfo, object[]>
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;
            var selector = GetPolicy<IMethodSelectorPolicy>(ref context, 
                context.RegistrationType, context.RegistrationName);

            foreach (var selection in selector.SelectMethods(ref context))
            {
                MethodInfo methodInfo;
                ParameterInfo[] parameters;
                object[] resolvers;

                switch (selection)
                {
                    case MethodInfo info:
                        methodInfo = info;
                        parameters = info.GetParameters();
                        resolvers = null;
                        break;

                    case MethodBaseMember<MethodInfo> methodBaseMember:
                        (methodInfo, resolvers) = methodBaseMember.FromType(context.Type);
                        parameters = methodInfo.GetParameters();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                // TODO: Consolidate validation
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
                                BuilderContextExpression.Existing,
                                dynamicBuildContext.TypeToBuild),
                            methodInfo,
                            BuildMethodParameterExpressions(parameters, resolvers))));
            }
        }

        #endregion


        #region Implementation

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
