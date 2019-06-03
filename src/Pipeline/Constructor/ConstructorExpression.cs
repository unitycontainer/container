using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        #region Fields

        private static readonly ConstructorInfo PerResolveInfo = typeof(InternalPerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(BuilderContextExpression.Context, 
                BuilderContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, BuilderContextExpression.Existing));

        protected static readonly Expression[] NoConstructorExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            BuilderContextExpression.Type))))};

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(ConstructorInfo info, object? resolvers)
        {
            var variable = Expression.Variable(info.DeclaringType);
            var parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);

            return Expression.IfThen(
                Expression.Equal(Expression.Constant(null), BuilderContextExpression.Existing),
                Expression.Block(new[] { variable }, new Expression[]
                {
                    Expression.Assign(variable, Expression.New(info, parametersExpr)),
                    Expression.Assign(BuilderContextExpression.Existing, Expression.Convert(variable, typeof(object)))
                }));
        }

        #endregion
    }
}
