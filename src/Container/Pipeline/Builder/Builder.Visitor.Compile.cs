using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public delegate T PipelineVisitor<T>(ref Pipeline_Builder<T> builder);


    public ref partial struct PipelineBuilder<TContext>
    {
        public static PipelineDelegate<TContext> CompileVisitor(IStagedStrategyChain strategies)
        {
            var chain = (IEnumerable<BuilderStrategy>)strategies;
            var lambda = Expression.Lambda<PipelineDelegate<TContext>>(
                Expression.Block(
                    Expression.Block(GetBuildUpExpressions(chain.GetEnumerator())),
                    PipelineBuilder<TContext>.Label),
                PipelineBuilder<TContext>.ContextExpression);

            return lambda.Compile();
        }
    }
}
