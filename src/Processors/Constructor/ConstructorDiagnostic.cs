using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public class ConstructorDiagnostic : ConstructorProcessor
    {
        #region Fields

        private static readonly Expression CannotConstructInterfaceExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructInterface),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)));

        private static readonly Expression CannotConstructAbstractClassExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructAbstractClass),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)));

        private static readonly Expression CannotConstructDelegateExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructDelegate),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)));

        private static readonly Expression TypeIsNotConstructableExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                 Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.TypeIsNotConstructable),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)));

        #endregion


        #region Constructors

        public ConstructorDiagnostic(IPolicySet policySet, Func<Type, bool> isTypeRegistered) 
            : base(policySet, isTypeRegistered)
        {
        }

        #endregion


        #region Expression Overrides

        public override IEnumerable<Expression> GetBuildSteps(Type type, IPolicySet registration)
        {
            // Validate if Type could be created
            var exceptionExpr = ValidateConstructedTypeExpression(type);
            if (null != exceptionExpr) return new[] { exceptionExpr };

            // Select ConstructorInfo
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();

            // Select appropriate ctor for the Type
            ConstructorInfo info;
            object[] resolvers = null;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    break;

                case MethodBaseMember<ConstructorInfo> injectionMember:
                    (info, resolvers) = injectionMember.FromType(type);
                    break;

                default:
                    return new[] { NoConstructorExpr };
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager)registration.Get(typeof(LifetimeManager));


            // Validate parameters
            var parameters = info.GetParameters();
            if (parameters.Any(p => p.ParameterType == info.DeclaringType))
            {
                if (null == lifetimeManager?.GetValue())
                {
                    return new Expression[]
                    {
                        Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                        Expression.Throw(
                            Expression.New(InvalidOperationExceptionCtor,
                            Expression.Call(
                                StringFormat,
                                Expression.Constant(Constants.SelectedConstructorHasRefItself),
                                Expression.Constant(info, typeof(ConstructorInfo)),
                                BuilderContextExpression.Type),
                            InvalidRegistrationExpression)))
                    };
                }
            }

            // Create 'new' expression
            var ifThenExpr = Expression.IfThen(
                Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                BuildMemberExpression(info, resolvers));

            // Check if PerResolveLifetimeManager is required
            return lifetimeManager is PerResolveLifetimeManager
                ? new[] { ifThenExpr, SetPerBuildSingletonExpr }
                : new Expression[] { ifThenExpr };
        }

        protected override Expression BuildMemberExpression(ConstructorInfo info, object[] resolvers)
        {
            // Check if had ByRef parameters
            var parameters = info.GetParameters();
            if (parameters.Any(pi => pi.ParameterType.IsByRef))
            {
                return Expression.Throw(Expression.New(InvalidOperationExceptionCtor,
                        Expression.Constant(CreateErrorMessage(Constants.SelectedConstructorHasRefParameters, info.DeclaringType, info)),
                        InvalidRegistrationExpression));
            }

            // Create 
            var variable = Expression.Variable(info.DeclaringType ?? throw new ArgumentNullException(nameof(info)));
            return Expression.Block(new[] { variable }, new Expression[]
            {
                Expression.Assign(variable, Expression.New(info, CreateParameterExpressions(info.GetParameters(), resolvers))),
                Expression.Assign(BuilderContextExpression.Existing, Expression.Convert(variable, typeof(object)))
            });


            // TODO: Check if required
            string CreateErrorMessage(string format, Type type, MethodBase constructor)
            {
                var parameterDescriptions =
                    constructor.GetParameters()
                        .Select(parameter => $"{parameter.ParameterType.FullName} {parameter.Name}");

                return string.Format(format, type.FullName, string.Join(", ", parameterDescriptions));
            }
        }

        #endregion

        #region Overrides

        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed)
        {
            // Validate if Type could be created
            var exception = ValidateConstructedTypeResolver(type);
            if (null != exception) return exception;

            // Select ConstructorInfo
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();

            // Select appropriate ctor for the Type
            ConstructorInfo info;
            object[] resolvers = null;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    break;

                case MethodBaseMember<ConstructorInfo> injectionMember:
                    (info, resolvers) = injectionMember.FromType(type);
                    break;

                default:
                    return (ref BuilderContext c) =>
                    {
                        if (null == c.Existing)
                            throw new InvalidOperationException($"No public constructor is available for type {c.Type}.",
                                new InvalidRegistrationException());

                        return c.Existing;
                    };
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager)registration.Get(typeof(LifetimeManager));

            // Validate parameters
            var parameters = info.GetParameters();
            if (parameters.Any(p => p.ParameterType == info.DeclaringType))
            {
                if (null == lifetimeManager?.GetValue())
                    return (ref BuilderContext c) =>
                    {
                        if (null == c.Existing)
                            throw new InvalidOperationException(string.Format(Constants.SelectedConstructorHasRefItself, info, c.Type),
                                new InvalidRegistrationException());

                        return c.Existing;
                    };
            }

            // Create dependency resolvers
            var parameterResolvers = CreateParameterResolvers(parameters, resolvers).ToArray();
            if (lifetimeManager is PerResolveLifetimeManager)
            {
                // PerResolve lifetime
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                    {
                        var dependencies = new object[parameterResolvers.Length];
                        for (var i = 0; i < dependencies.Length; i++)
                            dependencies[i] = parameterResolvers[i](ref c);

                        c.Existing = info.Invoke(dependencies);
                        c.Set(typeof(LifetimeManager),
                              new InternalPerResolveLifetimeManager(c.Existing));
                    }

                    return c.Existing;
                };
            }

            // Normal activator
            return (ref BuilderContext c) =>
            {
                if (null == c.Existing)
                {
                    var dependencies = new object[parameterResolvers.Length];
                    for (var i = 0; i < dependencies.Length; i++)
                        dependencies[i] = parameterResolvers[i](ref c);

                    c.Existing = info.Invoke(dependencies);
                }

                return c.Existing;
            };

        }

        #endregion


        #region Implementation

        private ResolveDelegate<BuilderContext> ValidateConstructedTypeResolver(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface)
#else
            if (type.IsInterface)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructInterface, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (type.IsAbstract)
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructAbstractClass, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (type.IsSubclassOf(typeof(Delegate)))
#endif
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.CannotConstructDelegate, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            if (type == typeof(string))
            {
                return (ref BuilderContext c) =>
                {
                    if (null == c.Existing)
                        throw new InvalidOperationException(string.Format(Constants.TypeIsNotConstructable, c.Type),
                            new InvalidRegistrationException());

                    return c.Existing;
                };
            }

            return null;
        }

        private Expression ValidateConstructedTypeExpression(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface)
#else
            if (type.IsInterface)
#endif
                return CannotConstructInterfaceExpr;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (type.IsAbstract)
#endif
                return CannotConstructAbstractClassExpr;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (type.IsSubclassOf(typeof(Delegate)))
#endif
                return CannotConstructDelegateExpr;

            if (type == typeof(string))
                return TypeIsNotConstructableExpr;

            return null;
        }

        #endregion

    }
}
