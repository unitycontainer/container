using Unity.Extension;

namespace Unity.Container
{
    public ref partial struct PipelineBuilder<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private BuilderStrategy[] _strategies;

        #endregion


        #region Constructors

        public PipelineBuilder(BuilderStrategy[] strategies)
        {
            _strategies = strategies;
        }

        #endregion
    }
}
