using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct RequestInfo
    {
        #region Fields

        public ErrorInfo Error;
        public ResolverOverride[] Overrides;

        #endregion


        #region Constructors

        public RequestInfo(ResolverOverride[] overrides)
        {
            Error = default;
            Overrides = overrides;
        }

        #endregion


        #region Public

        public bool IsFaulted => Error.IsFaulted;


        internal PerResolveOverride PerResolve
        {
            set
            {
                var position = Overrides.Length;
                Array.Resize(ref Overrides, Overrides.Length + 1);

                Overrides[position] = value;
            }
        }

        #endregion
    }
}
