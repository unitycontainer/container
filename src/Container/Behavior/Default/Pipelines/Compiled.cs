using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static ResolveDelegate<TContext> PipelineCompiled(ref TContext context)
        {
            return ((Policies<TContext>)context.Policies).ActivatePipeline;


            //var policies = (Policies<TContext>)context.Policies;
            //var chain = policies.TypeChain;

            //var factory = Analyse ??= chain.AnalyzePipeline<TContext>();

            //var analytics = factory(ref context);

            //var builder = new PipelineBuilder<TContext>(ref context);

            //return builder.CompilePipeline((object?[])analytics!);
        }


        public static PipelineFactory<TContext> DefaultCompileProcessorFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            return DefaultFactory;
        }

    }
}
