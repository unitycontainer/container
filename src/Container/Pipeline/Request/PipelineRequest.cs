using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineRequest
    {
        #region Fields

        public PipelineError Error;
        public ResolverOverride[] Overrides;

        #endregion


        #region Constructors

        public PipelineRequest(ResolverOverride[] overrides)
        {
            Error     = default;
            Overrides = overrides;
        }

        #endregion


        #region Public

        public bool IsFaulted => Error.IsFaulted;

        #endregion
    }
}
