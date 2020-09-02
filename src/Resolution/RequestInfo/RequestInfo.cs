using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Resolution
{
    public struct RequestInfo
    {
        #region Fields

        public readonly ResolverOverride[] Overrides; // TODO: nullable?

        #endregion


        #region Constructors

        public RequestInfo(ResolverOverride[] overrides)
        {
            Overrides = overrides;
        }

        #endregion
    }
}
