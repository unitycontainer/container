using System;
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

        public static ResolveDelegate<TContext> IteratedBuildUpPipelineFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
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

        public static ResolveDelegate<TContext> CompiledBuildUpPipelineFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            var logic = ExpressBuildUp(chain.Values.ToArray());
            var body = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, ContextExpression);

            return lambda.Compile();
        }

        public static ResolveDelegate<TContext> ResolvedBuildUpPipelineFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
            => ResolvedBuildUp(chain.Values.ToArray()) ?? ((ref TContext context) => UnityContainer.NoValue);
    }
}
