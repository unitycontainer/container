using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;


namespace Unity.Container
{
    [DebuggerDisplay("StagedChain: Type = {Type.Name}, Count = {Count}")]
    [DebuggerTypeProxy(typeof(StagedChainProxy))]
    public partial class StagedStrategyChain
    {
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


        #region Storage

        private struct Entry
        {
            public readonly BuilderStrategy? Strategy;

            public readonly bool Build;
            public readonly bool Express;
            public readonly MethodInfo? Analyse;
            public readonly MethodInfo? PreBuildUp;
            public readonly MethodInfo? PostBuildUp;
            
            public Expression?              AnalyseExpr;
            public IEnumerable<Expression>? PreBuildUpExpr;
            public IEnumerable<Expression>? PostBuildUpExpr;

            public Entry(BuilderStrategy strategy)
            {
                Strategy = strategy;
                Analyse = default;
                PreBuildUp = default;
                PostBuildUp = default;
                AnalyseExpr = default;
                PreBuildUpExpr  = default;
                PostBuildUpExpr = default;

                Build = false;
                Express = false;

                foreach (var method in strategy.GetType()
                                               .GetMethods())
                {
                    switch (method.Name)
                    {
                        case nameof(BuilderStrategy.BuildPipeline):
                            Build = !ReferenceEquals(typeof(BuilderStrategy), method!.DeclaringType);
                            break;
                        case nameof(BuilderStrategy.ExpressPipeline):
                            Express = !ReferenceEquals(typeof(BuilderStrategy), method!.DeclaringType);
                            break;

                        case nameof(BuilderStrategy.Analyse):
                            if (!ReferenceEquals(typeof(BuilderStrategy), method!.DeclaringType)) Analyse = method;
                            break;

                        case nameof(BuilderStrategy.PreBuildUp):
                            if (!ReferenceEquals(typeof(BuilderStrategy), method!.DeclaringType)) PreBuildUp = method;
                            break;

                        case nameof(BuilderStrategy.PostBuildUp):
                            if (!ReferenceEquals(typeof(BuilderStrategy), method!.DeclaringType)) PostBuildUp = method;
                            break;
                    }
                }
            }
        }

        #endregion


        #region Debug Proxy

        private class StagedChainProxy
        {
            public StagedChainProxy(StagedStrategyChain chain)
            {
                var names = Enum.GetNames(typeof(UnityBuildStage));

                Entries = new Entry[names.Length];

                for (int i = 0; i < names.Length; i++)
                {
                    ref var entry = ref Entries[i];
                    entry.Stage = names[i];
                    entry.Strategy = chain._stages[i].Strategy!;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Entry[] Entries { get; }

            [DebuggerDisplay("{Strategy}", Name = "{Stage,nq}")]
            public struct Entry
            {
                public string Stage;
                public BuilderStrategy Strategy;
            }
        }

        #endregion
    }
}
