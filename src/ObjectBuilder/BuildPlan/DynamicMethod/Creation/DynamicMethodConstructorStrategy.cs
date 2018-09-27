

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
using Unity.Storage;

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
            var info = typeof(DynamicMethodConstructorStrategy).GetTypeInfo();

            ThrowForNullExistingObjectMethod = info.GetDeclaredMethod(nameof(ThrowForNullExistingObject));
            ThrowForNullExistingObjectWithInvalidConstructorMethod = info.GetDeclaredMethod(nameof(ThrowForNullExistingObjectWithInvalidConstructor));
            ThrowForAttemptingToConstructInterfaceMethod = info.GetDeclaredMethod(nameof(ThrowForAttemptingToConstructInterface));
            ThrowForAttemptingToConstructAbstractClassMethod = info.GetDeclaredMethod(nameof(ThrowForAttemptingToConstructAbstractClass));
            ThrowForAttemptingToConstructDelegateMethod = info.GetDeclaredMethod(nameof(ThrowForAttemptingToConstructDelegate));
            SetCurrentOperationToResolvingParameterMethod = info.GetDeclaredMethod(nameof(SetCurrentOperationToResolvingParameter));
            SetCurrentOperationToInvokingConstructorMethod = info.GetDeclaredMethod(nameof(SetCurrentOperationToInvokingConstructor));
            SetPerBuildSingletonMethod = info.GetDeclaredMethod(nameof(SetPerBuildSingleton));
            ThrowForReferenceItselfConstructorMethod = info.GetDeclaredMethod(nameof(ThrowForReferenceItselfConstructor));

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

            string signature = CreateSignatureString(selectedConstructor.Constructor);

            if (selectedConstructor.Constructor.GetParameters().Any(pi => pi.ParameterType.IsByRef))
            {
                return CreateThrowForNullExistingObjectWithInvalidConstructor(buildContext, signature);
            }

            if (IsInvalidConstructor(targetTypeInfo, context, selectedConstructor))
            {
                return CreateThrowForReferenceItselfMethodConstructor(buildContext, signature);
            }

            return Expression.Block(CreateNewBuildupSequence(buildContext, selectedConstructor, signature));
        }


        private static bool IsInvalidConstructor(TypeInfo target, IBuilderContext context, SelectedConstructor selectedConstructor)
        {
            if (selectedConstructor.Constructor.GetParameters().Any(p => p.ParameterType.GetTypeInfo() == target))
            {
                var policy = (ILifetimePolicy)context.Policies.Get(context.BuildKey.Type, 
                                                                   context.BuildKey.Name, 
                                                                   typeof(ILifetimePolicy), out var _);
                if (null == policy?.GetValue())
                    return true;
            }

            return false;
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

        /// <summary>
        /// Build up the string that will represent the constructor signature
        /// in any exception message.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static string CreateSignatureString(ConstructorInfo constructor)
        {
            string typeName = (constructor ?? throw new ArgumentNullException(nameof(constructor))).DeclaringType.FullName;
            ParameterInfo[] parameters = constructor.GetParameters();
            string[] parameterDescriptions = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameterDescriptions[i] = string.Format(CultureInfo.CurrentCulture,
                    "{0} {1}",
                    parameters[i].ParameterType.FullName,
                    parameters[i].Name);
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}({1})",
                typeName,
                string.Join(", ", parameterDescriptions));
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

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an interface (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructInterface(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.CannotConstructInterface,
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an abstract class (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructAbstractClass(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.CannotConstructAbstractClass, 
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an delegate other than Func{T} or Func{IEnumerable{T}}.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructDelegate(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture, Constants.CannotConstructDelegate, 
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        public class InvalidRegistrationException : Exception
        {
            
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForNullExistingObject(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.NoConstructorFound,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved because of an invalid constructor.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        /// <param name="signature">The signature of the invalid constructor.</param>
        public static void ThrowForNullExistingObjectWithInvalidConstructor(IBuilderContext context, string signature)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.SelectedConstructorHasRefParameters,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name,
                    signature));
        }


        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved because of an invalid constructor.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        /// <param name="signature">The signature of the invalid constructor.</param>
        public static void ThrowForReferenceItselfConstructor(IBuilderContext context, string signature)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.SelectedConstructorHasRefItself,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name,
                    signature));
        }
    }
}
