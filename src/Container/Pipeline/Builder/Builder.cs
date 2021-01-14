using System.Collections.Generic;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private IEnumerator<BuilderStrategy> _enumerator;

        #endregion


        #region Constructors

        public PipelineBuilder(IStagedStrategyChain strategies)
        {
            _enumerator = ((IEnumerable<BuilderStrategy>)strategies).GetEnumerator();
        }

        #endregion
    }
}
