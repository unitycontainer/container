// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to call
    /// chosen methods (as specified by the current <see cref="IMethodSelectorPolicy"/>)
    /// as part of object build up.
    /// </summary>
    public class DynamicMethodCallStrategy : BuilderStrategy
    {
        private static readonly MethodInfo SetCurrentOperationToResolvingParameterMethod;
        private static readonly MethodInfo SetCurrentOperationToInvokingMethodInfo;

        static DynamicMethodCallStrategy()
        {
            var info = typeof(DynamicMethodCallStrategy).GetTypeInfo();

            SetCurrentOperationToResolvingParameterMethod =
                info.GetDeclaredMethod(nameof(SetCurrentOperationToResolvingParameter));

            SetCurrentOperationToInvokingMethodInfo =
                info.GetDeclaredMethod(nameof(SetCurrentOperationToInvokingMethod));
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override object PreBuildUp(IBuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)(context ?? throw new ArgumentNullException(nameof(context))).Existing;

            var selector = context.Policies.Get<IMethodSelectorPolicy>(context.OriginalBuildKey, out var resolverPolicyDestination);

            bool shouldClearOperation = false;

            foreach (SelectedMethod method in selector.SelectMethods(context, resolverPolicyDestination))
            {
                shouldClearOperation = true;

                string signatureString = GetMethodSignature(method.Method);

                GuardMethodIsNotOpenGeneric(method.Method);
                GuardMethodHasNoOutParams(method.Method);
                GuardMethodHasNoRefParams(method.Method);

                dynamicBuildContext.AddToBuildPlan(
                    Expression.Block(
                        Expression.Call(null, SetCurrentOperationToInvokingMethodInfo, Expression.Constant(signatureString), dynamicBuildContext.ContextParameter),
                        Expression.Call(
                            Expression.Convert(
                                dynamicBuildContext.GetExistingObjectExpression(),
                                dynamicBuildContext.TypeToBuild),
                            method.Method,
                            BuildMethodParameterExpressions(dynamicBuildContext, method, signatureString))));
            }

            // Clear the current operation
            if (shouldClearOperation)
            {
                dynamicBuildContext.AddToBuildPlan(dynamicBuildContext.GetClearCurrentOperationExpression());
            }

            return null;
        }

        private IEnumerable<Expression> BuildMethodParameterExpressions(DynamicBuildPlanGenerationContext context, SelectedMethod method, string methodSignature)
        {
            int i = 0;
            var methodParameters = method.Method.GetParameters();

            foreach (IResolverPolicy parameterResolver in method.GetParameterResolvers())
            {
                yield return context.CreateParameterExpression(
                                parameterResolver,
                                methodParameters[i].ParameterType,
                                Expression.Call(null,
                                    SetCurrentOperationToResolvingParameterMethod,
                                    Expression.Constant(methodParameters[i].Name, typeof(string)),
                                    Expression.Constant(methodSignature),
                                    context.ContextParameter));
                i++;
            }
        }

        private static void GuardMethodIsNotOpenGeneric(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectOpenGenericMethod, method);
            }
        }

        private static void GuardMethodHasNoOutParams(MethodInfo method)
        {
            if (method.GetParameters().Any(param => param.IsOut))
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void GuardMethodHasNoRefParams(MethodInfo method)
        {
            if (method.GetParameters().Any(param => param.ParameterType.IsByRef))
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void ThrowIllegalInjectionMethod(string format, MethodInfo method)
        {
            throw new IllegalInjectionMethodException(
                string.Format(CultureInfo.CurrentCulture,
                    format,
                    method.DeclaringType.GetTypeInfo().Name,
                    method.Name));
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        public static void SetCurrentOperationToResolvingParameter(string parameterName, string methodSignature, IBuilderContext context)
        {
            (context ?? throw new ArgumentNullException(nameof(context))).CurrentOperation = new MethodArgumentResolveOperation(
                context.BuildKey.Type,
                methodSignature, parameterName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        public static void SetCurrentOperationToInvokingMethod(string methodSignature, IBuilderContext context)
        {
            (context ?? throw new ArgumentNullException(nameof(context))).CurrentOperation = new InvokingMethodOperation(context.BuildKey.Type, methodSignature);
        }

        private static string GetMethodSignature(MethodBase method)
        {
            string methodName = method.Name;
            ParameterInfo[] parameterInfos = method.GetParameters();
            string[] parameterDescriptions = new string[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                parameterDescriptions[i] = parameterInfos[i].ParameterType.FullName + " " +
                    parameterInfos[i].Name;
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}({1})",
                methodName,
                string.Join(", ", parameterDescriptions));
        }
    }
}
