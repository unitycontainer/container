using System.Linq.Expressions;
using Unity.Extension;
using Unity.Processors;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static PipelineFactory<TContext> ChainToResolverIteratedFactory(MemberProcessor[] chain)
        {
            var delegates = chain
                .Where(p =>
                {
                    var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildResolver))!;
                    return typeof(MemberProcessor) != info.DeclaringType;
                })
                .Select(p => (BuilderStrategyDelegate<TContext>)p.BuildResolver)
                .ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                var pipeline = (BuilderStrategyDelegate<TContext>)(context.Existing ?? throw new InvalidOperationException());
                
                return (ref TContext context) =>
                {
                    pipeline(ref context);
                    return context.Existing;
                };
            };
        }

        public static PipelineFactory<TContext> ChainToResolverCompiledFactory(MemberProcessor[] chain)
        {
            var logic = ExpressChain(chain.Where(p =>
            {
                var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildResolver))!;
                return typeof(MemberProcessor) != info.DeclaringType;
            }).Select(p => (BuilderStrategyDelegate<TContext>)p.BuildResolver));

            var block = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(block, ContextExpression);
            var factory = lambda.Compile();

            return (ref TContext context) => factory;
        }

        public static PipelineFactory<TContext> ChainToResolverResolvedFactory(MemberProcessor[] chain)
            => ChainToResolverIteratedFactory(chain);
    }
}
