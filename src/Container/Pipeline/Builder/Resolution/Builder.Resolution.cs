using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext> : IBuildPipeline<TContext>
    {
        #region Fields


        #endregion


        #region IBuildPipeline

        public ResolveDelegate<TContext>? Build()
        {
            return _enumerator.MoveNext()
                 ? _enumerator.Current.Build<PipelineBuilder<TContext>, TContext>(ref this) ?? SeedMethod
                 : SeedMethod ;
        }

        #endregion



        #region Implementation

        #endregion
    }
}
