using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines
    {
        public static ResolveDelegate<BuilderContext> PipelineFromStagedChainFactory(IStagedStrategyChain strategies)
        {
            var processors = ((IEnumerable<BuilderStrategy>)strategies).ToArray();

            return (ref BuilderContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }

        // TODO: Compiled chain
        public static ResolveDelegate<BuilderContext> CompiledChainFactory(IStagedStrategyChain strategies)
            =>  new PipelineBuilder<BuilderContext>((IEnumerable<BuilderStrategy>)strategies).ExpressBuildUp();
    }
}
