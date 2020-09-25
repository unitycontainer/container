using Unity.Resolution;

namespace Unity.Container
{
    public struct RequestInfo
    {
        #region Fields

        public bool    IsFaulted;
        public string? Error;
        public ResolverOverride[] Overrides;

        #endregion


        #region Constructors

        public RequestInfo(ResolverOverride[] overrides)
        {
            Error = null;
            IsFaulted = false;
            Overrides = overrides;
        }

        #endregion
    }
}
