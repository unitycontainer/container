using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public ref partial struct PipelineBuilder<TContext>
    {
        #region Fields

        private static readonly ParameterInfo _contextParameter 
            = typeof(PipelineDelegate<TContext>).GetMethod(nameof(PipelineDelegate<TContext>.Invoke))!
                                                .GetParameters()[0];
        
        private static readonly ParameterExpression ContextExpression 
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        private static readonly MemberExpression IsFaultedExpression =
            Expression.MakeMemberAccess(ContextExpression,
                typeof(TContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        private static readonly LabelTarget ExitLabel = Expression.Label();
        private static readonly LabelExpression Label = Expression.Label(ExitLabel);
        private static readonly ConditionalExpression IfThenExpression 
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));

        #endregion

        public static PipelineDelegate<TContext> BuildUpPipeline(IEnumerable<BuilderStrategy> chain)
        {
            var lambda = Expression.Lambda<PipelineDelegate<TContext>>(
                Expression.Block(
                    Expression.Block(GetExpressionsBuildUP(chain.GetEnumerator(), true)), 
                    Label), 
                ContextExpression);

            return lambda.Compile();
        }

        private static IEnumerable<Expression> GetExpressionsBuildUP<T>(T enumerator, bool skip = false)
            where T : IEnumerator<BuilderStrategy>
        {
            if (!enumerator.MoveNext()) yield break;

            var strategy = enumerator.Current;

            // PreBuildUP
            var buildUp = GetExpressionBuildUP(strategy, nameof(BuilderStrategy.PreBuildUp));
            if (buildUp is not null) yield return buildUp;

            // Everything in the middle
            foreach (var expression in GetExpressionsBuildUP(enumerator))
                yield return expression;

            // PostBuildUP
            buildUp = GetExpressionBuildUP(strategy, nameof(BuilderStrategy.PostBuildUp), skip);
            if (buildUp is not null) yield return buildUp;
        }


        private static Expression? GetExpressionBuildUP(BuilderStrategy strategy, string name, bool skip = false)
        {
            var method = strategy.GetType().GetMethod(name)!;

            if (ReferenceEquals(method.DeclaringType, typeof(BuilderStrategy)))
                return null;

            return skip 
                ? Expression.Call(Expression.Constant(strategy), method.MakeGenericMethod(typeof(TContext)), ContextExpression)
                : Expression.Block(
                    Expression.Call(Expression.Constant(strategy), method.MakeGenericMethod(typeof(TContext)), ContextExpression), 
                    IfThenExpression );
        }
    }
}
