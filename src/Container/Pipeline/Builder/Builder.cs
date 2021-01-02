using System.Collections.Generic;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private IEnumerator<BuilderStrategy> _enumerator;

        #endregion


        #region Constructors

        public PipelineBuilder(IEnumerable<BuilderStrategy> strategies)
        {
            _enumerator = strategies.GetEnumerator();
        }

        #endregion
    }
}
