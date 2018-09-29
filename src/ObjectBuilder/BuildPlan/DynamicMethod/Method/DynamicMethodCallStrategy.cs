using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Expressions;
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

            foreach (var method in selector.SelectMethods(ref context))
            {
                var methodInfo = method.Method;
                var parameters = methodInfo.GetParameters();

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
                            method.Method,
                            BuildMethodParameterExpressions<TBuilderContext>(dynamicBuildContext, method, parameters))));
            }
        }

        #endregion


        #region Implementation

        private IEnumerable<Expression> BuildMethodParameterExpressions<TBuilderContext>(DynamicBuildPlanGenerationContext context, 
            SelectedMethod method, ParameterInfo[] methodParameters)
            where TBuilderContext : IBuilderContext
        {
            int i = 0;

            foreach (var parameterResolver in method.GetParameterResolvers())
            {
                yield return context.CreateParameterExpression<TBuilderContext>(
                                parameterResolver,
                                methodParameters[i].ParameterType,
                                Expression.Assign(
                                    BuilderContextExpression<TBuilderContext>.CurrentOperation,
                                    Expression.Constant(methodParameters[i].Member, typeof(object))));
                i++;
            }
        }

        #endregion
    }
}
