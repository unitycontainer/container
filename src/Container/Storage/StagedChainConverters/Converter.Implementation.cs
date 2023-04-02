using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class ChainConverter<TTarget, TContext>
        where TContext : IBuildPlanContext<TTarget>
    {
        #region Expressions

        private static readonly ParameterInfo _contextParameter
            = typeof(BuildPlanDelegate<TTarget, TContext>).GetMethod(nameof(BuildPlanDelegate<TTarget, TContext>.Invoke))!
                                       .GetParameters()[0];

        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        public static readonly MemberExpression IsFaultedExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(IBuildPlanContext).GetProperty(nameof(IBuildPlanContext.IsFaulted))!);

        public static readonly MemberExpression TargetExpression
            = Expression.MakeMemberAccess(ContextExpression, typeof(IBuildPlanContext<TTarget>).GetProperty(
                    nameof(IBuildPlanContext<TTarget>.Target))!);

        public static readonly LabelTarget ExitLabel = Expression.Label();
        public static readonly LabelExpression Label = Expression.Label(ExitLabel);

        private static readonly ConditionalExpression IfThenExpression
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));

        #endregion


        #region Implementation

        public static BuildPlanDelegate<TTarget, TContext> ChainToFactory(IEnumerable<BuildPlanDelegate<TTarget, TContext>> chain)
        {
            var logic = ExpressChain(chain);
            var block = Expression.Block(Expression.Block(logic), Label, TargetExpression);
            var lambda = Expression.Lambda<BuildPlanDelegate<TTarget, TContext>>(block, ContextExpression);
            return lambda.Compile();
        }

        private static IEnumerable<Expression> ExpressChain(IEnumerable<BuildPlanDelegate<TTarget, TContext>> chain)
        {
            foreach (var strategy in chain)
            {
                yield return Expression.Invoke(Expression.Constant(strategy), ContextExpression);

                yield return Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));
            }
        }

        #endregion
    }
}
