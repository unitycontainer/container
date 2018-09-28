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
using Unity.Container.Lifetime;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that emits IL to call constructors
    /// as part of creating a build plan.
    /// </summary>
    public class DynamicMethodConstructorStrategy : BuilderStrategy
    {
        private static readonly MethodInfo ThrowForNullExistingObjectMethod;
        private static readonly MethodInfo ThrowForNullExistingObjectWithInvalidConstructorMethod;
        private static readonly MethodInfo ThrowForAttemptingToConstructInterfaceMethod;
        private static readonly MethodInfo ThrowForAttemptingToConstructAbstractClassMethod;
        private static readonly MethodInfo ThrowForAttemptingToConstructDelegateMethod;
        private static readonly MethodInfo SetCurrentOperationToResolvingParameterMethod;
        private static readonly MethodInfo SetCurrentOperationToInvokingConstructorMethod;
        private static readonly MethodInfo SetPerBuildSingletonMethod;
        private static readonly MethodInfo ThrowForReferenceItselfConstructorMethod;

        static DynamicMethodConstructorStrategy()
        {
            var objectBuilderExceptionsInfo = typeof(ObjectBuilderExceptions).GetTypeInfo();

            ThrowForNullExistingObjectMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForNullExistingObject));
            ThrowForNullExistingObjectWithInvalidConstructorMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForNullExistingObjectWithInvalidConstructor));
            ThrowForAttemptingToConstructInterfaceMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForAttemptingToConstructInterface));
            ThrowForAttemptingToConstructAbstractClassMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForAttemptingToConstructAbstractClass));
            ThrowForAttemptingToConstructDelegateMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForAttemptingToConstructDelegate));
            ThrowForReferenceItselfConstructorMethod = objectBuilderExceptionsInfo.GetDeclaredMethod(nameof(ObjectBuilderExceptions.ThrowForReferenceItselfConstructor));

            var info = typeof(DynamicMethodConstructorStrategy).GetTypeInfo();

            SetCurrentOperationToResolvingParameterMethod = info.GetDeclaredMethod(nameof(SetCurrentOperationToResolvingParameter));
            SetCurrentOperationToInvokingConstructorMethod = info.GetDeclaredMethod(nameof(SetCurrentOperationToInvokingConstructor));
            SetPerBuildSingletonMethod = info.GetDeclaredMethod(nameof(SetPerBuildSingleton));
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <remarks>Existing object is an instance of <see cref="DynamicBuildPlanGenerationContext"/>.</remarks>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            DynamicBuildPlanGenerationContext buildContext =
                (DynamicBuildPlanGenerationContext)(context ?? throw new ArgumentNullException(nameof(context))).Existing;

            GuardTypeIsNonPrimitive(context);

            buildContext.AddToBuildPlan(
                 Expression.IfThen(
                        Expression.Equal(
                            buildContext.GetExistingObjectExpression(),
                            Expression.Constant(null)),
                            CreateInstanceBuildupExpression(buildContext, context)));

            var policy = context.Policies.Get(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(ILifetimePolicy), out _);
            if (policy is PerResolveLifetimeManager)
            {
                buildContext.AddToBuildPlan(
                    Expression.Call(null, SetPerBuildSingletonMethod, buildContext.ContextParameter));
            }
        }

        internal Expression CreateInstanceBuildupExpression(DynamicBuildPlanGenerationContext buildContext, IBuilderContext context)
        {
            var targetTypeInfo = context.BuildKey.Type.GetTypeInfo();

            if (targetTypeInfo.IsInterface)
            {
                return CreateThrowWithContext(buildContext, ThrowForAttemptingToConstructInterfaceMethod);
            }

            if (targetTypeInfo.IsAbstract)
            {
                return CreateThrowWithContext(buildContext, ThrowForAttemptingToConstructAbstractClassMethod);
            }

            if (targetTypeInfo.IsSubclassOf(typeof(Delegate)))
            {
                return CreateThrowWithContext(buildContext, ThrowForAttemptingToConstructDelegateMethod);
            }

            IConstructorSelectorPolicy selector =
                context.Policies.GetPolicy<IConstructorSelectorPolicy>(context.OriginalBuildKey, out var resolverPolicyDestination);

            SelectedConstructor selectedConstructor = selector.SelectConstructor(context, resolverPolicyDestination);

            if (selectedConstructor == null)
            {
                return CreateThrowWithContext(buildContext, ThrowForNullExistingObjectMethod);
            }

            string signature = ConstructorHelper.CreateSignatureString(selectedConstructor.Constructor);

            if (selectedConstructor.Constructor.GetParameters().Any(pi => pi.ParameterType.IsByRef))
            {
                return CreateThrowForNullExistingObjectWithInvalidConstructor(buildContext, signature);
            }

            if (ConstructorHelper.IsInvalidConstructor(targetTypeInfo, context, selectedConstructor))
            {
                return CreateThrowForReferenceItselfMethodConstructor(buildContext, signature);
            }

            return Expression.Block(CreateNewBuildupSequence(buildContext, selectedConstructor, signature));
        }

        private static Expression CreateThrowWithContext(DynamicBuildPlanGenerationContext buildContext, MethodInfo throwMethod)
        {
            return Expression.Call(
                                null,
                                throwMethod,
                                buildContext.ContextParameter);
        }

        private static Expression CreateThrowForNullExistingObjectWithInvalidConstructor(DynamicBuildPlanGenerationContext buildContext, string signature)
        {
            return Expression.Call(
                                null,
                                ThrowForNullExistingObjectWithInvalidConstructorMethod,
                                buildContext.ContextParameter,
                                Expression.Constant(signature, typeof(string)));
        }

        private static Expression CreateThrowForReferenceItselfMethodConstructor(DynamicBuildPlanGenerationContext buildContext, string signature)
        {
            return Expression.Call(
                                null,
                                ThrowForReferenceItselfConstructorMethod,
                                buildContext.ContextParameter,
                                Expression.Constant(signature, typeof(string)));
        }

        private IEnumerable<Expression> CreateNewBuildupSequence(DynamicBuildPlanGenerationContext buildContext, SelectedConstructor selectedConstructor, string signature)
        {
            var parameterExpressions = BuildConstructionParameterExpressions(buildContext, selectedConstructor, signature);
            var newItemExpression = Expression.Variable(selectedConstructor.Constructor.DeclaringType, "newItem");

            yield return Expression.Call(null,
                                        SetCurrentOperationToInvokingConstructorMethod,
                                        Expression.Constant(signature),
                                        buildContext.ContextParameter,
                                        Expression.Constant(selectedConstructor.Constructor.DeclaringType));

            yield return Expression.Assign(
                            buildContext.GetExistingObjectExpression(),
                            Expression.Convert(
                                Expression.New(selectedConstructor.Constructor, parameterExpressions),
                                typeof(object)));

            yield return buildContext.GetClearCurrentOperationExpression();
        }

        private IEnumerable<Expression> BuildConstructionParameterExpressions(DynamicBuildPlanGenerationContext buildContext, SelectedConstructor selectedConstructor, string constructorSignature)
        {
            int i = 0;
            var constructionParameters = selectedConstructor.Constructor.GetParameters();

            foreach (IResolverPolicy parameterResolver in selectedConstructor.GetParameterResolvers())
            {
                yield return buildContext.CreateParameterExpression(
                                parameterResolver,
                                constructionParameters[i].ParameterType,
                                Expression.Call(null,
                                                SetCurrentOperationToResolvingParameterMethod,
                                                Expression.Constant(constructionParameters[i].Name, typeof(string)),
                                                Expression.Constant(constructorSignature),
                                                buildContext.ContextParameter,
                                                Expression.Constant(selectedConstructor.Constructor.DeclaringType)));
                i++;
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to set up a PerResolveLifetimeManager lifetime manager
        /// if the current object is such.
        /// </summary>
        /// <param name="context">Current build context.</param>
        public static void SetPerBuildSingleton(IBuilderContext context)
        {
            var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
            context.Policies.Set(context.OriginalBuildKey.Type,
                                 context.OriginalBuildKey.Name,
                                 typeof(ILifetimePolicy), perBuildLifetime);
        }

        // Verify the type we're trying to build is actually constructable -
        // CLR primitive types like string and int aren't.
        private static void GuardTypeIsNonPrimitive(IBuilderContext context)
        {
            var typeToBuild = context.BuildKey.Type;
            if (!typeToBuild.GetTypeInfo().IsInterface)
            {
                if (ReferenceEquals(typeToBuild, typeof(string)))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Constants.TypeIsNotConstructable,
                            typeToBuild.GetTypeInfo().Name));
                }
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        public static void SetCurrentOperationToResolvingParameter(string parameterName, string constructorSignature, IBuilderContext context, Type type)
        {
            context.CurrentOperation = new ConstructorArgumentResolveOperation(type, constructorSignature, parameterName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        public static void SetCurrentOperationToInvokingConstructor(string constructorSignature, IBuilderContext context, Type type)
        {
            context.CurrentOperation = new InvokingConstructorOperation(type, constructorSignature);
        }
    }
}
