using System.Collections.Generic;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private IEnumerator<BuilderStrategy> _enumerator;
        private ResolveDelegate<TContext>? SeedMethod;

        #endregion


        #region Constructors

        public PipelineBuilder(IEnumerable<BuilderStrategy> strategies)
        {
            SeedMethod = null;

            _enumerator = strategies.GetEnumerator();
        }

        #endregion


        #region Properties

        public BuilderStrategy Strategy => _enumerator.Current;

        #endregion
    }
}
