using System.Linq;
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

    }
}
