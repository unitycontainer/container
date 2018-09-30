using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Expressions;
using Unity.Injection;
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
        #region Static Fields

        private static readonly TypeInfo TypeInfo = typeof(DynamicMethodConstructorStrategy).GetTypeInfo();
        private static readonly Expression InvalidRegistrationExpression = Expression.New(typeof(InvalidRegistrationException));
        private static readonly MethodInfo StringFormat = typeof(string).GetTypeInfo() 
                                                                        .DeclaredMethods
                                                                        .First(m =>
                                                                        {
                                                                            var parameters = m.GetParameters();
                                                                            return m.Name == nameof(string.Format) &&
                                                                                   m.GetParameters().Length == 2 &&
                                                                                   typeof(object) == parameters[1].ParameterType;
                                                                        });

        #endregion


        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <remarks>Existing object is an instance of <see cref="DynamicBuildPlanGenerationContext"/>.</remarks>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            // Verify the type we're trying to build is actually constructable -
            // CLR primitive types like string and int aren't.
            if (!context.TypeInfo.IsInterface)
            {
                if (context.Type == typeof(string))
                {
                    throw new InvalidOperationException(
                        $"The type {context.TypeInfo.Name} cannot be constructed. You must configure the container to supply this value.");
                }
            }

            DynamicBuildPlanGenerationContext buildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            buildContext.AddToBuildPlan(
                Expression.IfThen(
                    Expression.Equal(
                        BuilderContextExpression<TBuilderContext>.Existing,
                        Expression.Constant(null)),
                    CreateInstanceBuildupExpression(buildContext, ref context)));

            var policy = context.Policies.Get(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(ILifetimePolicy));
            if (policy is PerResolveLifetimeManager)
            {
                // TODO: Optimize
                var setPerBuildSingletonMethod = TypeInfo.GetDeclaredMethod(nameof(SetPerBuildSingleton)).MakeGenericMethod(typeof(TBuilderContext));

                buildContext.AddToBuildPlan(
                    Expression.Call(null, setPerBuildSingletonMethod, BuilderContextExpression<TBuilderContext>.Context));
            }
        }

        #endregion


        #region Implementation

        internal Expression CreateInstanceBuildupExpression<TBuilderContext>(DynamicBuildPlanGenerationContext buildContext, ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            // Validate type can be constructed

            if (context.TypeInfo.IsInterface)
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructInterface),
                            BuildContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            if (context.TypeInfo.IsAbstract)
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructAbstractClass),
                            BuildContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            if (context.TypeInfo.IsSubclassOf(typeof(Delegate)))
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructDelegate),
                            BuildContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            if (context.Type == typeof(string))
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.TypeIsNotConstructable),
                            BuildContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            var selector = context.Policies.GetPolicy<IConstructorSelectorPolicy>(
                context.OriginalBuildKey.Type, context.OriginalBuildKey.Name);

            if (null == selector)
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant("No appropriate constructor selector is registered for type {0}."),
                            BuildContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            SelectedConstructor selectedConstructor;
            switch (selector.SelectConstructor(ref context))
            {
                case ConstructorInfo info:
                    selectedConstructor = new SelectedConstructor(info);
                    break;

                //case InjectionConstructor injectionConstructor:
                //    selectedConstructor = new SelectedConstructor(injectionConstructor);
                //    break;

                case SelectedConstructor selectedCtor:
                    selectedConstructor = selectedCtor;
                    break;

                default:
                    return Expression.Throw(
                        Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                            Expression.Call(StringFormat,
                                Expression.Constant(Constants.NoConstructorFound),
                                BuildContextExpression<TBuilderContext>.Type),
                            InvalidRegistrationExpression));
            }

            var parameters = selectedConstructor.Constructor.GetParameters();
            if (parameters.Any(pi => pi.ParameterType.IsByRef))
            {
                return Expression.IfThen(Expression.Equal(BuildContextExpression<TBuilderContext>.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefParameters, context.Type, selectedConstructor.Constructor)),
                        InvalidRegistrationExpression)));
            }

            if (IsInvalidConstructor(context.TypeInfo, ref context, parameters))
            {
                return Expression.IfThen(Expression.Equal(BuildContextExpression<TBuilderContext>.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefItself, context.Type, selectedConstructor.Constructor)),
                        InvalidRegistrationExpression)));
            }

            var parameterExpressions = BuildConstructionParameterExpressions<TBuilderContext>(buildContext, selectedConstructor, parameters);

            return Expression.Assign(BuilderContextExpression<TBuilderContext>.Existing,
                Expression.Convert(Expression.New(selectedConstructor.Constructor, parameterExpressions), typeof(object))
            );
        }

        private static bool IsInvalidConstructor<TBuilderContext>(TypeInfo target, ref TBuilderContext context, ParameterInfo[] parameters)
            where TBuilderContext : IBuilderContext
        {
            if (parameters.Any(p => Equals(p.ParameterType.GetTypeInfo(), target)))
            {
                var policy = (ILifetimePolicy)context.Policies.Get(context.BuildKey.Type,
                    context.BuildKey.Name,
                    typeof(ILifetimePolicy));
                if (null == policy?.GetValue())
                    return true;
            }

            return false;
        }

        private static string CreateErrorMessage(string format, Type type, MethodBase constructor)
        {
            var parameterDescriptions =
                constructor.GetParameters()
                    .Select(parameter => $"{parameter.ParameterType.FullName} {parameter.Name}");

            return string.Format(format, type.FullName, string.Join(", ", parameterDescriptions));
        }

        private IEnumerable<Expression> BuildConstructionParameterExpressions<TBuilderContext>(DynamicBuildPlanGenerationContext buildContext, 
            SelectedMemberWithParameters selectedConstructor, ParameterInfo[] parameters) where TBuilderContext : IBuilderContext
        {
            var i = 0;

            foreach (var parameterResolver in selectedConstructor.GetParameterResolvers())
            {
                var parameter = parameters[i];
                yield return buildContext.CreateParameterExpression<TBuilderContext>(
                    parameterResolver,
                    parameter.ParameterType,
                    Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation,
                        Expression.Constant(parameter, typeof(object))));
                i++;
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to set up a PerResolveLifetimeManager lifetime manager
        /// if the current object is such.
        /// </summary>
        /// <param name="context">Current build context.</param>
        public static void SetPerBuildSingleton<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
            context.Policies.Set(context.OriginalBuildKey.Type,
                                 context.OriginalBuildKey.Name,
                                 typeof(ILifetimePolicy), perBuildLifetime);
        }

        #endregion
    }
}
