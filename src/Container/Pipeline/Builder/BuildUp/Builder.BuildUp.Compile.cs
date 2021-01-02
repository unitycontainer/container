using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
    {

        public static ResolveDelegate<TContext> CompileBuildUp(IStagedStrategyChain strategies)
        {
            var chain = (IEnumerable<BuilderStrategy>)strategies;
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(
                Expression.Block(
                    Expression.Block(GetBuildUpExpressions(chain.GetEnumerator())),
                    PipelineBuilder<TContext>.Label,
                    TargetExpression),
                PipelineBuilder<TContext>.ContextExpression);

            return lambda.Compile();
        }
    }
}
