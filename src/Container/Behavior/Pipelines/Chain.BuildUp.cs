using System.Linq.Expressions;
using Unity.Extension;
using Unity.Processors;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static PipelineFactory<TContext> ChainToBuildUpIteratedFactory(MemberProcessor[] chain)
        {
            var delegates = chain
                .Where(p =>
                    {
                        var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                        return typeof(MemberProcessor) != info.DeclaringType;
                    })
                .Select(p => (BuilderStrategyDelegate<TContext>)p.BuildUp)
                .ToArray();

            return (ref TContext context) =>
            {
                return (ref TContext context) =>
                {
                    var i = -1;

                    while (!context.IsFaulted && ++i < delegates.Length)
                        delegates[i](ref context);
                    
                    return context.Existing;
                };
            };
        }

        public static PipelineFactory<TContext> ChainToBuildUpCompiledFactory(MemberProcessor[] chain)
        {
            var logic = ExpressChain(chain.Where(p =>
                                          {
                                              var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                                              return typeof(MemberProcessor) != info.DeclaringType;
                                          })
                                          .Select(p => (BuilderStrategyDelegate<TContext>)p.BuildUp));

            var block = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(block, ContextExpression);
            var factory = lambda.Compile();

            return (ref TContext context) => factory;
        }

        public static PipelineFactory<TContext> ChainToBuildUpResolvedFactory(MemberProcessor[] chain) 
            => ChainToBuildUpIteratedFactory(chain);
    }
}
