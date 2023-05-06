using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class ChainConverter<TTarget>
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

        public static BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>> ChainToFactory(IEnumerable<BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>>> chain)
        {
            var logic = ExpressChain(chain);
            var block = Expression.Block(Expression.Block(logic), Label, BuildPlanContext<TTarget>.TargetExpression);
            var lambda = Expression.Lambda<BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>>>(block, BuildPlanContext<TTarget>.ContextExpression);
            return lambda.Compile();
        }

        private static IEnumerable<Expression> ExpressChain(IEnumerable<BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>>> chain)
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
