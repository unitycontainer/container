using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder.Expressions;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Builder.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that emits IL to call constructors
    /// as part of creating a build plan.
    /// </summary>
    public class CompiledConstructorStrategy : CompiledStrategy<ConstructorInfo, object[]>
    {
        #region Static Fields

        private static readonly TypeInfo TypeInfo = typeof(CompiledConstructorStrategy).GetTypeInfo();
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
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (!context.Type.GetTypeInfo().IsInterface)
#else
            if (!context.Type.IsInterface)
#endif
            {
                    if (context.Type == typeof(string))
                {
                    throw new InvalidOperationException(
                        $"The type {context.Type.Name} cannot be constructed. You must configure the container to supply this value.");
                }
            }

            DynamicBuildPlanGenerationContext buildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            buildContext.AddToBuildPlan(
                Expression.IfThen(Expression.Equal(BuilderContextExpression<TBuilderContext>.Existing, Expression.Constant(null)), 
                    ValidateConstructedType(ref context) ?? 
                    CreateInstanceBuildupExpression(ref context)));

            var policy = context.Policies.Get(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name, typeof(LifetimeManager));
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

        private Expression ValidateConstructedType<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = context.Type.GetTypeInfo();
            if (typeInfo.IsInterface)
#else
            if (context.Type.IsInterface)
#endif
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructInterface),
                            IResolveContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (context.Type.IsAbstract)
#endif
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructAbstractClass),
                            IResolveContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (context.Type.IsSubclassOf(typeof(Delegate)))
#endif
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructDelegate),
                            IResolveContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            if (context.Type == typeof(string))
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.TypeIsNotConstructable),
                            IResolveContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            return null;
        }

        private Expression ValidateSelectedConstructor<TBuilderContext>(ref TBuilderContext context, ConstructorInfo ctor)
            where TBuilderContext : IBuilderContext
        {
            var parameters = ctor.GetParameters();
            if (parameters.Any(pi => pi.ParameterType.IsByRef))
            {
                return Expression.IfThen(Expression.Equal(BuilderContextExpression<TBuilderContext>.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefParameters, context.Type, ctor)),
                        InvalidRegistrationExpression)));
            }

            if (IsInvalidConstructor(context.Type, ref context, parameters))
            {
                return Expression.IfThen(Expression.Equal(BuilderContextExpression<TBuilderContext>.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefItself, context.Type, ctor)),
                        InvalidRegistrationExpression)));
            }

            return null;
        }

        private Expression CreateInstanceBuildupExpression<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            var selector = context.Policies.GetPolicy<IConstructorSelectorPolicy>(
                context.OriginalBuildKey.Type, context.OriginalBuildKey.Name);

            if (null == selector)
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant("No appropriate constructor selector is registered for type {0}."),
                            IResolveContextExpression<TBuilderContext>.Type),
                        InvalidRegistrationExpression));
            }

            switch (selector.SelectConstructor(ref context))
            {
                case MethodBaseMember<ConstructorInfo> methodBaseMember:
                    var (ctor, resolvers) = methodBaseMember.FromType(context.Type);
                    return ValidateSelectedConstructor(ref context, ctor) ??
                           Expression.Assign(
                               BuilderContextExpression<TBuilderContext>.Existing,
                               Expression.Convert(
                                   Expression.New(
                                       ctor,
                                       BuilderContextExpression<TBuilderContext>.GetParameters(ctor, resolvers)),
                                   typeof(object)));

                case ConstructorInfo info:
                    return ValidateSelectedConstructor(ref context, info) ??
                           Expression.Assign(
                               BuilderContextExpression<TBuilderContext>.Existing, 
                               Expression.Convert(
                                   Expression.New(
                                       info, 
                                       BuilderContextExpression<TBuilderContext>.GetParameters(info)), 
                                   typeof(object)));

                case SelectedConstructor selected:
                    return ValidateSelectedConstructor(ref context, selected.Constructor) ??
                           Expression.Assign(
                               BuilderContextExpression<TBuilderContext>.Existing, 
                               Expression.Convert(
                                   Expression.New(
                                       selected.Constructor, 
                                       BuilderContextExpression<TBuilderContext>.GetParameters(selected)),
                                   typeof(object)));
                default:
                    return Expression.Throw(
                        Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                            Expression.Call(StringFormat,
                                Expression.Constant(Constants.NoConstructorFound),
                                IResolveContextExpression<TBuilderContext>.Type),
                            InvalidRegistrationExpression));
            }
        }

        private static bool IsInvalidConstructor<TBuilderContext>(Type target, ref TBuilderContext context, ParameterInfo[] parameters)
            where TBuilderContext : IBuilderContext
        {
            if (parameters.Any(p => p.ParameterType == target))
            {
                var policy = (LifetimeManager)context.Policies.Get(
                    context.Type,
                    context.Name,
                    typeof(LifetimeManager));
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
                                 typeof(LifetimeManager), perBuildLifetime);
        }

#endregion
    }
}
