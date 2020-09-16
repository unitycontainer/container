using Unity.Resolution;

namespace Unity.Container
{
    public struct RequestInfo
    {
        #region Fields

        public readonly ResolverOverride[] Overrides;

        #endregion


        #region Constructors

        public RequestInfo(ResolverOverride[] overrides)
        {
            Overrides = overrides;
        }

        #endregion
    }
}
