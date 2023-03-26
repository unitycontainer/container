using System.Linq.Expressions;
using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static ResolveDelegate<TContext> IteratedChainToPipelineFactory(BuilderStrategyDelegate<TContext>[] processors)
        {
            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i](ref context);

                return context.Existing;
            };
        }

        public static ResolveDelegate<TContext> CompiledChainToPipelineFactory(BuilderStrategyDelegate<TContext>[] processors)
        {
            var logic  = ExpressChain(processors);
            var block  = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(block, ContextExpression);

            return lambda.Compile();
        }

        public static ResolveDelegate<TContext> ResolvedChainToPipelineFactory(BuilderStrategyDelegate<TContext>[] processors)
            => IteratedChainToPipelineFactory(processors);
    }
}
