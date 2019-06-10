using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Lifetime;

namespace Unity
{
    public partial class ConstructorPipeline
    {
        #region Fields

        private static readonly ConstructorInfo PerResolveInfo = typeof(RuntimePerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(PipelineContextExpression.Context, 
                PipelineContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(PerResolveInfo, PipelineContextExpression.Existing));

        protected static readonly Expression[] NoConstructorExpr = new [] {
            Expression.IfThen(Expression.Equal(Expression.Constant(null), PipelineContextExpression.Existing),
                Expression.Throw(
                    Expression.New(InvalidRegistrationExpressionCtor,
                        Expression.Call(StringFormat,
                            Expression.Constant("No public constructor is available for type {0}."),
                            PipelineContextExpression.Type))))};

        #endregion


        #region Overrides

        protected override Expression GetResolverExpression(ConstructorInfo info, object? resolvers)
        {
            var variable = Expression.Variable(info.DeclaringType);
            var parametersExpr = CreateParameterExpressions(info.GetParameters(), resolvers);

            return Expression.IfThen(
                Expression.Equal(Expression.Constant(null), PipelineContextExpression.Existing),
                Expression.Block(new[] { variable }, new Expression[]
                {
                    Expression.Assign(variable, Expression.New(info, parametersExpr)),
                    Expression.Assign(PipelineContextExpression.Existing, Expression.Convert(variable, typeof(object)))
                }));
        }

        #endregion
    }
}
