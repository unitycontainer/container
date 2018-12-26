using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Container.Lifetime;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public class ConstructorProcessor : MethodBaseInfoProcessor<ConstructorInfo>
    {
        #region Fields

        private static readonly MethodInfo SetPerBuildSingletonMethod = typeof(ConstructorProcessor)
            .GetTypeInfo().GetDeclaredMethod(nameof(SetPerBuildSingleton));

        private static readonly ConstructorLengthComparer ConstructorComparer = new ConstructorLengthComparer();

        #endregion


        #region Constructors

        public ConstructorProcessor()
            : base(typeof(InjectionConstructorAttribute))
        {

        }

        #endregion


        #region Overrides

        protected override ConstructorInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                       .ToArray();
#endif
        }

        /// <summary>
        /// Selects default constructor
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be built</param>
        /// <param name="members">All public constructors this type implements</param>
        /// <returns></returns>
        protected override object GetDefault(Type type, ConstructorInfo[] members)
        {
            Array.Sort(members, ConstructorComparer);

            switch (members.Length)
            {
                case 0:
                    return null;

                case 1:
                    return members[0];

                default:
                    var paramLength = members[0].GetParameters().Length;
                    if (members[1].GetParameters().Length == paramLength)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Constants.AmbiguousInjectionConstructor,
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return members[0];
            }
        }

        public override IEnumerable<Expression> GetEnumerator(ref BuilderContext context)
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

            var selector = GetPolicy<ISelect<ConstructorInfo>>(ref context,
                context.RegistrationType, context.RegistrationName);

            if (null == selector)
            {
                return new[] { Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant("No appropriate constructor selector is registered for type {0}."),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression))};
            }

            Expression ctorExpr;

            switch (selector.Select(ref context)
                            .FirstOrDefault())
            {
                case MethodBaseMember<ConstructorInfo> methodBaseMember:
                    var (ctor, resolvers) = methodBaseMember.FromType(context.Type);
                    ctorExpr = ValidateSelectedConstructor(ref context, ctor) ??
                               Expression.Assign(context.Variable,
                                   Expression.New(ctor, CreateParameterExpressions(ctor.GetParameters(), resolvers)));
                    break;

                case ConstructorInfo info:
                    ctorExpr = ValidateSelectedConstructor(ref context, info) ??
                               Expression.Assign(context.Variable,
                                   Expression.New(info, CreateParameterExpressions(info.GetParameters(), null)));
                    break;

                default:
                    ctorExpr = Expression.Throw(
                        Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                            Expression.Call(StringFormat,
                                Expression.Constant(Constants.NoConstructorFound),
                                BuilderContextExpression.Type),
                            InvalidRegistrationExpression));
                    break;
            }

            var createExpr = Expression.IfThenElse(Expression.NotEqual(Expression.Constant(null), BuilderContextExpression.Existing),
                    Expression.Assign(context.Variable, Expression.Convert(BuilderContextExpression.Existing, context.Type)),
                    ValidateConstructedType(ref context) ?? ctorExpr);

            return context.Registration.Get(typeof(LifetimeManager)) is PerResolveLifetimeManager
                ? new Expression[] { createExpr, BuilderContextExpression.SetPerBuildSingleton(ref context) }
                : new Expression[] { createExpr };
        }

        protected override Expression CreateExpression(ConstructorInfo info, object[] resolvers, ParameterExpression variable)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Implementation

        private Expression ValidateConstructedType(ref BuilderContext context)
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
                            BuilderContextExpression.Type),
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
                            BuilderContextExpression.Type),
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
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

            if (context.Type == typeof(string))
            {
                return Expression.Throw(
                    Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.TypeIsNotConstructable),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

            return null;
        }

        private Expression ValidateSelectedConstructor(ref BuilderContext context, ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            if (parameters.Any(pi => pi.ParameterType.IsByRef))
            {
                return Expression.IfThen(Expression.Equal(BuilderContextExpression.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefParameters, context.Type, ctor)),
                        InvalidRegistrationExpression)));
            }

            if (IsInvalidConstructor(context.Type, ref context, parameters))
            {
                return Expression.IfThen(Expression.Equal(BuilderContextExpression.Existing, Expression.Constant(null)),
                    Expression.Throw(Expression.New(ExceptionExpression.InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefItself, context.Type, ctor)),
                        InvalidRegistrationExpression)));
            }

            return null;
        }

        private static bool IsInvalidConstructor(Type target, ref BuilderContext context, ParameterInfo[] parameters)
        {
            if (parameters.Any(p => p.ParameterType == target))
            {
                var policy = (LifetimeManager)context.Get(context.Type, context.Name, typeof(LifetimeManager));
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

        public static void SetPerBuildSingleton(ref BuilderContext context)
        {
            var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
            context.Set(context.RegistrationType, context.RegistrationName, typeof(LifetimeManager), perBuildLifetime);
        }

        #endregion


        #region Nested Types

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            public int Compare(ConstructorInfo x, ConstructorInfo y) => y?.GetParameters().Length ?? 0 - x?.GetParameters().Length ?? 0;
        }

        #endregion
    }
}
