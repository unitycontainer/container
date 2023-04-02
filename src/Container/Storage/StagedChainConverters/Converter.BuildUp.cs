using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Processors;

namespace Unity.Container
{
    internal static partial class BuildUpChainConverter
    {
        #region Expressions

        private static readonly ParameterInfo _contextParameter
            = typeof(BuilderStrategyDelegate<BuilderContext>).GetMethod(nameof(BuilderStrategyDelegate<BuilderContext>.Invoke))!
                                                .GetParameters()[0];

        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        public static readonly MemberExpression IsFaultedExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(BuilderContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        public static readonly MemberExpression ExistingExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(BuilderContext).GetProperty(nameof(IBuilderContext.Existing))!);

        public static readonly LabelTarget ExitLabel = Expression.Label();
        public static readonly LabelExpression Label = Expression.Label(ExitLabel);

        private static readonly ConditionalExpression IfThenExpression
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));

        #endregion


        #region Resolvers

        public static ResolverPipeline ChainToIteratedBuildUpPipeline(FactoryBuilderStrategy[] delegates)
        {
            return (ref BuilderContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                return context.Existing;
            };
        }

        public static ResolverPipeline ChainToCompiledBuildUpPipeline(FactoryBuilderStrategy[] delegates)
        {
            var logic = ExpressChain(delegates);
            var block = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolverPipeline>(block, ContextExpression);

            return lambda.Compile();
        }

        public static ResolverPipeline ChainToResolvedBuildUpPipeline(FactoryBuilderStrategy[] delegates)
            => ChainToIteratedBuildUpPipeline(delegates);

        #endregion


        #region Factories

        public static FactoryPipeline ChainToBuildUpIteratedFactory(MemberProcessor[] chain)
        {
            var delegates = chain
                .Where(p =>
                {
                    var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                    return typeof(MemberProcessor) != info.DeclaringType;
                })
                .Select(p => (FactoryBuilderStrategy)p.BuildUp)
                .ToArray();

            return (ref BuilderContext context) =>
            {
                return (ref BuilderContext context) =>
                {
                    var i = -1;

                    while (!context.IsFaulted && ++i < delegates.Length)
                        delegates[i](ref context);

                    return context.Existing;
                };
            };
        }

        public static FactoryPipeline ChainToBuildUpResolvedFactory(MemberProcessor[] chain)
            => ChainToBuildUpIteratedFactory(chain);

        public static FactoryPipeline ChainToBuildUpCompiledFactory(MemberProcessor[] chain)
        {
            var logic = ExpressChain(chain.Where(p =>
            {
                var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                return typeof(MemberProcessor) != info.DeclaringType;
            })
            .Select(p => (FactoryBuilderStrategy)p.BuildUp));

            var block = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolverPipeline>(block, ContextExpression);
            var factory = lambda.Compile();

            return (ref BuilderContext context) => factory;
        }

        #endregion


        #region Implementation

        private static IEnumerable<Expression> ExpressChain(IEnumerable<FactoryBuilderStrategy> chain)
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
