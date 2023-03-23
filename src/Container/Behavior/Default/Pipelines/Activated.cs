using System;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        public static ResolveDelegate<TContext> PipelineActivated(ref TContext context)
        {
            return ((Policies<TContext>)context.Policies).ActivatePipeline;
        }

        public static ResolveDelegate<TContext> ActivatePipelineFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            throw new NotImplementedException();
            //return ((Policies<BuilderContext>)context.Policies).ActivatePipeline;
        }

    }
}
