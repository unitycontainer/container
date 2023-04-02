using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class ChainConverter<TTarget, TContext>
        where TContext : IBuildPlanContext<TTarget>
    {
        #region Expressions

        public static readonly LabelTarget ExitLabel = Expression.Label();
        public static readonly LabelExpression Label = Expression.Label(ExitLabel);

        private static readonly ConditionalExpression IfThenExpression
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), BuildPlanContext<TTarget>.IsFaultedExpression),
                    Expression.Return(ExitLabel));

        #endregion


        #region Implementation

        public static BuildPlanStrategyDelegate<TTarget, TContext> ChainToFactory(IEnumerable<BuildPlanStrategyDelegate<TTarget, TContext>> chain)
        {
            var logic = ExpressChain(chain);
            var block = Expression.Block(Expression.Block(logic), Label, BuildPlanContext<TTarget>.TargetExpression);
            var lambda = Expression.Lambda<BuildPlanStrategyDelegate<TTarget, TContext>>(block, BuildPlanContext<TTarget>.ContextExpression);
            return lambda.Compile();
        }

        private static IEnumerable<Expression> ExpressChain(IEnumerable<BuildPlanStrategyDelegate<TTarget, TContext>> chain)
        {
            foreach (var strategy in chain)
            {
                yield return Expression.Invoke(Expression.Constant(strategy), BuildPlanContext<TTarget>.ContextExpression);
                yield return IfThenExpression;
            }
        }

        #endregion
    }
}
