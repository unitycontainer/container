using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor 
    {
        #region Fields

        private static readonly MethodInfo SetMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Set))
                .First(m => 2 == m.GetParameters().Length);

        private static readonly ConstructorInfo PerResolveInfo = typeof(InternalPerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        private static readonly ConstructorInfo InvalidOperationExceptionCtor =
            typeof(InvalidOperationException)
                .GetTypeInfo()
                .DeclaredConstructors
                .First(c =>
                {
                    var parameters = c.GetParameters();
                    return 2 == parameters.Length &&
                           typeof(string) == parameters[0].ParameterType &&
                           typeof(Exception) == parameters[1].ParameterType;
                });

        private static readonly Expression NoConstructorExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                    Expression.Throw(
                        Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(StringFormat,
                        Expression.Constant("No public constructor is available for type {0}."),
                        BuilderContextExpression.Type),
                    InvalidRegistrationExpression)));

        private static readonly Expression SetPerBuildSingletonExpr = 
            Expression.Call(BuilderContextExpression.Context, SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, BuilderContextExpression.Existing));

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


        #region Overrides

        public override IEnumerable<Expression> GetBuildSteps(Type type, IPolicySet registration)
        {
            // Validate if Type could be created
            var exceptionExpr = ValidateConstructedTypeExpression(type);
            if (null != exceptionExpr) return new[] { exceptionExpr };

            // Select ConstructorInfo
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();

            // Validate constructor info
            if (null == selection) return new[] { NoConstructorExpr };


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
                    throw new InvalidOperationException($"Unknown constructor representation");
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
            return registration.Get(typeof(LifetimeManager)) is PerResolveLifetimeManager
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
            var variable = Expression.Variable(info.DeclaringType);
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


        #region Implementation

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
