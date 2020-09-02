using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Resolution
{
    public struct RequestRoot
    {
        #region Fields

        public readonly ResolverOverride[] Overrides; // TODO: nullable?

        #endregion


        #region Constructors

        public RequestRoot(ResolverOverride[] overrides)
        {
            Overrides = overrides;
        }

        #endregion
    }
}
