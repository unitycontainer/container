using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        #region Expression Constants

        private static IEnumerable<Expression> EmptyExpression = Enumerable.Empty<Expression>();
        private static ConstantExpression NullConstant = Expression.Constant(null);
        private static readonly ParameterInfo _contextParameter
            = typeof(PipelineDelegate<TContext>).GetMethod(nameof(PipelineDelegate<TContext>.Invoke))!
                                                .GetParameters()[0];
        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        public static readonly MemberExpression IsFaultedExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(TContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        public static readonly MemberExpression ExistingExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(TContext).GetProperty(nameof(IBuilderContext.Existing))!);

        public static readonly LabelTarget ExitLabel = Expression.Label();
        public static readonly LabelExpression Label = Expression.Label(ExitLabel);

        private static readonly ConditionalExpression IfThenExpression
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));

        #endregion


        #region Implementation


        private static IEnumerable<Expression> Express(BuilderStrategyDelegate<TContext>[] chain)
        {
            foreach (var strategy in chain)
            {
                yield return Expression.Invoke(Expression.Constant(strategy), ContextExpression);

                yield return Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));
            }
        }

        private static IEnumerable<Expression> ExpressBuildUp(BuilderStrategy[] chain, int level = 0)
        {
            if (chain.Length <= level) return EmptyExpression;

            var strategy = chain[level];
            var type = strategy.GetType();

            var preBuildUpMethod = type.GetMethod(nameof(BuilderStrategy.PreBuildUp))!;
            var pre = !ReferenceEquals(typeof(BuilderStrategy), preBuildUpMethod.DeclaringType);

            var postBuildUpMethod = type.GetMethod(nameof(BuilderStrategy.PostBuildUp))!;
            var post = !ReferenceEquals(typeof(BuilderStrategy), postBuildUpMethod.DeclaringType);

            if (pre && post)
            {
                return ExpressBuildUp(preBuildUpMethod, strategy)
                    .Concat(ExpressBuildUp(chain, level + 1))
                    .Concat(ExpressBuildUp(postBuildUpMethod, strategy));
            }
            else if (pre)
            {
                return ExpressBuildUp(preBuildUpMethod, strategy)
                    .Concat(ExpressBuildUp(chain, level + 1));
            }
            else if (post)
            {
                return ExpressBuildUp(chain, level + 1)
                    .Concat(ExpressBuildUp(postBuildUpMethod, strategy));
            }

            return ExpressBuildUp(chain, level + 1);
        }

        private static IEnumerable<Expression> ExpressBuildUp(MethodInfo method, BuilderStrategy strategy)
        {
            return new Expression[]
            {
                Expression.Call(Expression.Constant(strategy),
                    method.MakeGenericMethod(typeof(TContext)),
                    ContextExpression),

                Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel))
            };
        }

        #endregion
    }
}
