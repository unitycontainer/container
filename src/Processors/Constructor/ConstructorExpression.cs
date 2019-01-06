using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Lifetime;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor 
    {
        #region Fields

        protected static readonly ConstructorInfo InvalidOperationExceptionCtor =
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

        private static readonly ConstructorInfo PerResolveInfo = typeof(InternalPerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(BuilderContextExpression.Context, 
                BuilderContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, BuilderContextExpression.Existing));

        protected static readonly Expression NoConstructorExpr =
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Throw(
                    Expression.New(InvalidOperationExceptionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            BuilderContextExpression.Type),
                        InvalidRegistrationExpression)));

        #endregion


        #region Overrides

        public override IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration)
        {
            // Select ConstructorInfo
            var selector = GetPolicy<ISelect<ConstructorInfo>>(registration);
            var selection = selector.Select(type, registration)
                                    .FirstOrDefault();

            // Select appropriate ctor for the Type
            ConstructorInfo info;
            IEnumerable<Expression> parametersExpr;

            switch (selection)
            {
                case ConstructorInfo memberInfo:
                    info = memberInfo;
                    parametersExpr = CreateParameterExpressions(info.GetParameters());
                    break;

                case MethodBase<ConstructorInfo> injectionMember:
                    object[] resolvers;
                    (info, resolvers) = injectionMember.FromType(type);
                    parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);
                    break;

                default:
                    return new[] { NoConstructorExpr };
            }

            // Get lifetime manager
            var lifetimeManager = (LifetimeManager)registration.Get(typeof(LifetimeManager));

            // Create 'new' expression
            var variable = Expression.Variable(info.DeclaringType);
            var ifThenExpr = Expression.IfThen(
                Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Block(new[] { variable }, new Expression[]
                {
                    Expression.Assign(variable, Expression.New(info, parametersExpr)),
                    Expression.Assign(BuilderContextExpression.Existing, Expression.Convert(variable, typeof(object)))
                }));

            // Check if PerResolveLifetimeManager is required
            return lifetimeManager is PerResolveLifetimeManager
                ? new[] { ifThenExpr, SetPerBuildSingletonExpr }
                : new Expression[] { ifThenExpr };
        }

        #endregion
    }
}
