using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        #region Fields

        private static ResolveDelegate<TContext>? Analyse;

        #endregion


        public static ResolveDelegate<TContext> PipelineResolved(ref TContext context)
        {
            //var policies = (Policies<TContext>)context.Policies;
            //var chain = policies.StrategiesChain;

            //var factory = Analyse;// ??= chain.AnalyzePipeline<TContext>();

            //var analytics = factory(ref context);

            //var builder = new PipelineBuilder<TContext>(ref context);

            //return builder.BuildPipeline((object?[])analytics!);
            return (ref TContext c) => null;

        }
    }
}
