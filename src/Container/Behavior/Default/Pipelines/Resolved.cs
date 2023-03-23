using System;
using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        #region Fields

        private static ResolveDelegate<BuilderContext>? Analyse;

        #endregion


        public static ResolveDelegate<TContext> PipelineResolved(ref TContext context)
        {
            return ((Policies<TContext>)context.Policies).ActivatePipeline;

            //var policies = (Policies<TContext>)context.Policies;
            //var chain    = policies.TypeChain;

            //var factory  = Analyse ??= chain.AnalyzePipeline<TContext>();

            //var analytics = factory(ref context);

            //var builder = new PipelineBuilder<TContext>(ref context);

            //return builder.BuildPipeline((object?[])analytics!);
        }
    }
}
