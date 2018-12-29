using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : MethodBaseInfoProcessor<ConstructorInfo>
    {
        #region Fields

        private static readonly MethodInfo SetMethod =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Set))
                .First(m => 2 == m.GetParameters().Length);

        private static readonly ConstructorLengthComparer ConstructorComparer = new ConstructorLengthComparer();

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

        private static readonly Expression NoConstructorExceptionExpr =
            Expression.Throw(
                Expression.New(InvalidOperationExceptionCtor,
                    Expression.Call(StringFormat,
                        Expression.Constant("No public constructor is available for type {0}."),
                        BuilderContextExpression.Type),
                    InvalidRegistrationExpression));

        private static readonly Expression SetPerBuildSingletonExpr = 
            Expression.Call(BuilderContextExpression.Context, SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, BuilderContextExpression.Existing));

        #endregion


        #region Overrides

        public override IEnumerable<Expression> GetBuildSteps(Type type, IPolicySet registration)
        {
            var newExpr = base.GetBuildSteps(type, registration)
                              .FirstOrDefault() ?? NoConstructorExceptionExpr;

            var IfThenExpr = Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                    ValidateConstructedTypeExpression(type) ?? newExpr);

            return registration.Get(typeof(LifetimeManager)) is PerResolveLifetimeManager
                ? new Expression[] { IfThenExpr, SetPerBuildSingletonExpr }
                : new Expression[] { IfThenExpr };
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
            {
                return Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructInterface),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsAbstract)
#else
            if (type.IsAbstract)
#endif
            {
                return Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructAbstractClass),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeInfo.IsSubclassOf(typeof(Delegate)))
#else
            if (type.IsSubclassOf(typeof(Delegate)))
#endif
            {
                return Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.CannotConstructDelegate),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

            if (type == typeof(string))
            {
                return Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(
                            StringFormat,
                            Expression.Constant(Constants.TypeIsNotConstructable),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression));
            }

            return null;
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
