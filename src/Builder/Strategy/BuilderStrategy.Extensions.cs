using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;


namespace Unity.Strategies
{
    /// <summary>
    /// Several utilities to build strategy chins
    /// </summary>
    public static class BuilderStrategyExtensions
    {
        // TODO: Remove unused
        public static ResolveDelegate<TContext> AnalyzePipeline<TContext>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> that)
            where TContext : IBuilderContext
        {
            var chain = (StagedStrategyChain<BuilderStrategy, UnityBuildStage>)that;
            var version = chain.Version;
            var processors = chain.Values
                                  .Select(p => (ResolveDelegate<TContext>)p.Analyze)
                                  .ToArray();

            return (ref TContext context) =>
            {
                var analysis = new object?[processors.Length + 1];
                analysis[processors.Length] = version;

                for (var i = 0; i < processors.Length; i++)
                {
                    var @delegate = processors[i];
                    if (@delegate is null) continue;

                    analysis[i] = @delegate(ref context);
                }

                return analysis;
            };
        }

        public static ResolveDelegate<TContext> AnalyzeCompiled<TContext>(this StagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
            where TContext : IBuilderContext
        {
            var body = Expression.Block(Expression.NewArrayInit(typeof(object), chain.Analyze<TContext>()));
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, Context<TContext>.ContextExpression);

            return lambda.Compile();
        }

        private static IEnumerable<Expression> Analyze<TContext>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> that)
            where TContext : IBuilderContext
        {
            var chain = (StagedStrategyChain<BuilderStrategy, UnityBuildStage>)that;
            var version = chain.Version;
            var processors = chain.Values
                                  .Select(p => (ResolveDelegate<TContext>)p.Analyze)
                                  .ToArray();
            //foreach (ResolveDelegate<TContext> method in chain.Values
            //                                                  .Select(p => (ResolveDelegate<TContext>)p.Analyse))
            //{
            //    yield return Expression.Call(Expression.Constant(strategy),
            //        method.MakeGenericMethod(typeof(TContext)),
            //        Context<TContext>.ContextExpression);

            //}

            //var strategies = chain.Values.ToArray();


            //for (var i = 0; i < chain.Count; i++)
            //{
            //    var strategy = strategies[i];
            //    if (strategy is null) continue;

            //    var method = _stages[i].Analyse;

            //    yield return method is null
            //        ? NullConstant
            //        : _stages[i].AnalyseExpr ??= Expression.Call(Expression.Constant(strategy),
            //        method.MakeGenericMethod(typeof(TContext)),
            //        Context<TContext>.ContextExpression);
            //}

            yield return Expression.Constant(version, typeof(object));
        }


        #region Expressions

        private static class Context<TContext> where TContext : IBuilderContext
        {
            private static readonly ParameterInfo _contextParameter
                = typeof(PipelineDelegate<TContext>).GetMethod(nameof(PipelineDelegate<TContext>.Invoke))!
                                                    .GetParameters()[0];

            #region Properties

            public static readonly ParameterExpression ContextExpression
                = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

            public static readonly MemberExpression IsFaultedExpression
                = Expression.MakeMemberAccess(ContextExpression,
                    typeof(TContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

            public static readonly MemberExpression ExistingExpression
                = Expression.MakeMemberAccess(ContextExpression,
                    typeof(TContext).GetProperty(nameof(IBuilderContext.Existing))!);

            #endregion


            #region API

            public static readonly LabelTarget ExitLabel = Expression.Label();
            public static readonly LabelExpression Label = Expression.Label(ExitLabel);

            private static readonly ConditionalExpression IfThenExpression
                = Expression.IfThen(
                        Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                        Expression.Return(ExitLabel));

            #endregion
        }

        #endregion
    }
}
