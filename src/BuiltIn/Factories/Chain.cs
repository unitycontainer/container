using System.Collections.Generic;
using System.Linq;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        #region Factory

        public static ResolveDelegate<PipelineContext> ChainPipelineFactory(IStagedStrategyChain strategies)
        {
            var processors = ((IEnumerable<BuilderStrategy>)strategies).ToArray();

            return (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }


        public static ResolveDelegate<PipelineContext> CompiledChainFactory(IStagedStrategyChain strategies)
            =>  new PipelineBuilder<PipelineContext>((IEnumerable<BuilderStrategy>)strategies).ExpressBuildUp();
        

        #endregion
    }
}
