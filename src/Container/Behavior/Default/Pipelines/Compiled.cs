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
        public static ResolveDelegate<TContext> PipelineCompiled(ref TContext context)
        {
            
            var policies = (Policies<TContext>)context.Policies;
            var chain = policies.TypeChain;

            var factory = Analyse ??= chain.AnalyzePipeline<TContext>();

            var analytics = factory(ref context);

            var builder = new PipelineBuilder<TContext>(ref context);

            return builder.CompilePipeline((object?[])analytics!);
        }

        public static ResolveDelegate<TContext> CompiledBuildUpPipelineFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            var logic = ExpressBuildUp(chain.Values.ToArray());
            var body  = Expression.Block(Expression.Block(logic), Label, ExistingExpression);
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(body, ContextExpression);

            return lambda.Compile();
        }
    }
}
