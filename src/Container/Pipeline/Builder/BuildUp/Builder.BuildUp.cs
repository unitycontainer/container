using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
    {
        #region Default Chain Factory

        public static ResolveDelegate<TContext> BuildUp(IStagedStrategyChain strategies)
        {
            var processors = ((IEnumerable<BuilderStrategy>)strategies).ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }

        #endregion
    }
}
