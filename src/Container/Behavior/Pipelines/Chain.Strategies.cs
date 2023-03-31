using System.Linq.Expressions;
using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static ResolveDelegate<TContext> ChainToStrategiesIteratedFactory(BuilderStrategyDelegate<TContext>[] delegates)
        {
            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                return context.Existing;
            };
        }

        public static ResolveDelegate<TContext> ChainToStrategiesCompiledFactory(BuilderStrategyDelegate<TContext>[] delegates)
        {
            var logic  = ExpressChain(delegates);
            var block  = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(block, ContextExpression);

            return lambda.Compile();
        }

        public static ResolveDelegate<TContext> ChainToStrategiesResolvedFactory(BuilderStrategyDelegate<TContext>[] delegates)
            => ChainToStrategiesIteratedFactory(delegates);
    }
}
