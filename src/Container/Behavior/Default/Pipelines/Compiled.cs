using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineCompiled(ref BuilderContext context)
        {
            var policies = (Policies<BuilderContext>)context.Policies;
            var chain = policies.TypeChain;

            var factory = Analyse ??= chain.AnalyzePipeline<BuilderContext>();

            var analytics = factory(ref context);

            var builder = new PipelineBuilder<BuilderContext>(ref context);

            return builder.CompilePipeline((object?[])analytics!);
        }
    }
}
