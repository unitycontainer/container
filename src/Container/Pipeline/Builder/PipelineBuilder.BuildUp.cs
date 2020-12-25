using System;
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
            = typeof(Extension.PipelineDelegate<TContext>).GetMethod(nameof(Extension.PipelineDelegate<TContext>.Invoke))!
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

        public static Extension.PipelineDelegate<TContext> BuildUpPipeline(IEnumerable<BuilderStrategy> chain)
        {
            var lambda = Expression.Lambda<Extension.PipelineDelegate<TContext>>(
                Expression.Block(
                    Expression.Block(GetExpressionsBuildUP(chain.GetEnumerator())),
                    PipelineBuilder<TContext>.Label),
                PipelineBuilder<TContext>.ContextExpression);

            return lambda.Compile();
        }

        private static IEnumerable<Expression> GetExpressionsBuildUP<T>(T enumerator)
            where T : IEnumerator<BuilderStrategy>
        {
            if (!enumerator.MoveNext()) yield break;

            var strategy = enumerator.Current;
            var type = strategy.GetType();

            // PreBuildUP
            var block = GetBuildUPBlock(strategy, type.GetMethod(nameof(BuilderStrategy.PreBuildUp))!);
            if (block is not null) yield return block;

            // Everything in the middle
            foreach (var expression in GetExpressionsBuildUP(enumerator))
                yield return expression;

            // PostBuildUP
            block = GetBuildUPBlock(strategy, type.GetMethod(nameof(BuilderStrategy.PostBuildUp))!);
            if (block is not null) yield return block;
        }


        private static Expression? GetBuildUPBlock(BuilderStrategy strategy, MethodInfo method)
        {
            if (ReferenceEquals(method.DeclaringType, typeof(BuilderStrategy))) return null;

            return Expression.Block(
                    Expression.Call(Expression.Constant(strategy), method.MakeGenericMethod(typeof(TContext)), ContextExpression), 
                    IfThenExpression );
        }
    }
}
