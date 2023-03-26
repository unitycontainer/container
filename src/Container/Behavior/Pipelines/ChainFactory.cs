using System.Linq;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static ResolveDelegate<TContext> IteratedChainToPipelineFactory(IStagedStrategyChain sender)
        {
            var chain = (IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>)sender;
            var processors = chain.Values.ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i](ref context);

                return context.Existing;
            };
        }

        public static ResolveDelegate<TContext> CompiledChainToPipelineFactory(IStagedStrategyChain sender)
        {
            var chain  = (IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>)sender;
            var logic  = ExpressChain(chain.Values.ToArray());
            var block  = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(block, ContextExpression);

            return lambda.Compile();
        }

        public static ResolveDelegate<TContext> ResolvedChainToPipelineFactory(IStagedStrategyChain chain)
            => IteratedChainToPipelineFactory(chain);
    }
}
