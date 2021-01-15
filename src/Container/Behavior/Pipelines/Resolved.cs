using System;
using System.ComponentModel.Composition;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        #region Fields

        private static ResolveDelegate<BuilderContext>? Analyse;

        #endregion


        public static ResolveDelegate<BuilderContext> PipelineResolved(ref BuilderContext context)
        {
            var policies = (Policies<BuilderContext>)context.Policies;
            var chain = policies.TypeChain;

            if (Analyse is null)
            {
                Analyse = chain.AnalysePipeline<BuilderContext>();
            }

            var analytics = Analyse(ref context);

            var builder = new PipelineBuilder<BuilderContext>(chain);

            return builder.Build(ref context) ?? UnityContainer.DummyPipeline;
        }
    }
}
