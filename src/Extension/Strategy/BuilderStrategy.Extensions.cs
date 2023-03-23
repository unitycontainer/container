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


        public static ResolveDelegate<TContext> BuildUpPipeline<TContext>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
            where TContext : IBuilderContext
        {
            var processors = chain.Values.ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Existing;
            };
        }

        public static ResolveDelegate<TContext> BuildUpCompiled<TContext>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
            where TContext : IBuilderContext
        {
            var strategies = chain.Values.ToArray();
            var exec = Expression.Block(strategies.BuildUp<TContext>(0));
            var body = Expression.Block(exec, Context<TContext>.Label, Context<TContext>.ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, Context<TContext>.ContextExpression);

            return lambda.Compile();
        }


        private static IEnumerable<Expression> BuildUp<TContext>(this BuilderStrategy[] chain, int level)
            where TContext : IBuilderContext
        {
            if (chain.Length <= level) return EmptyExpression;

            var strategy = chain[level];

            throw new NotImplementedException();

            //if (strategy is null) return BuildUp<TContext>(level + 1);

            //if (entry.PreBuildUp is not null && entry.PostBuildUp is not null)
            //{
            //    return entry.PreBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PreBuildUp, strategy)
            //        .Concat(BuildUp<TContext>(level + 1))
            //        .Concat(entry.PostBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PostBuildUp, strategy));
            //}

            //if (entry.PreBuildUp is not null)
            //{
            //    return entry.PreBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PreBuildUp, strategy)
            //        .Concat(BuildUp<TContext>(level + 1));
            //}

            //if (entry.PostBuildUp is not null)
            //{
            //    return BuildUp<TContext>(level + 1)
            //        .Concat(entry.PostBuildUpExpr ??= ExpressBuildUp<TContext>(entry.PostBuildUp!, strategy));
            //}

            //return BuildUp<TContext>(level + 1);
        }


        private static IEnumerable<Expression> ExpressBuildUp<TContext>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain, MethodInfo method, BuilderStrategy strategy)
            where TContext : IBuilderContext
        {
            return new Expression[]
            {
                Expression.Call(Expression.Constant(strategy),
                    method.MakeGenericMethod(typeof(TContext)),
                    Context<TContext>.ContextExpression),

                Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), Context<TContext>.IsFaultedExpression),
                    Expression.Return(Context<TContext>.ExitLabel))
            };
        }


        #region Expressions

        private static IEnumerable<Expression> EmptyExpression = Enumerable.Empty<Expression>();
        private static ConstantExpression NullConstant = Expression.Constant(null);

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
