using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class StagedStrategyChain
    {

        public ResolveDelegate<TContext> AnalysePipeline<TContext>()
            where TContext : IBuilderContext
        {
            var version = Version;
            var processors = new ResolveDelegate<TContext>?[Count];
            
            var index = 0;
            for (var i = 0; i < _size; i++)
            {
                ref var entry = ref _stages[i];
                if (entry.Strategy is null) continue;

                processors[index++] = entry.Analyse is null
                    ? null : entry.Strategy!.Analyse;
            }

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

        public ResolveDelegate<TContext> AnalyseCompiled<TContext>()
            where TContext : IBuilderContext
        {
            var body  = Expression.Block(Expression.NewArrayInit(typeof(object), Analyse<TContext>()));
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, Context<TContext>.ContextExpression);

            return lambda.Compile();
        }

        private IEnumerable<Expression> Analyse<TContext>()
            where TContext : IBuilderContext
        {
            for (var i = 0; i < _size; i++)
            {
                var strategy = _stages[i].Strategy;
                if (strategy is null) continue;

                var method = _stages[i].Analyse;

                yield return method is null
                    ? NullConstant
                    : _stages[i].AnalyseExpr ??= Expression.Call(Expression.Constant(strategy),
                    method.MakeGenericMethod(typeof(TContext)),
                    Context<TContext>.ContextExpression);
            }

            yield return Expression.Constant(Version, typeof(object));
        }

    }
}
