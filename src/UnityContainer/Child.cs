using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constructors

        protected UnityContainer(UnityContainer parent)
        {
            // Extension Management
            _context    = parent._context;
            _extensions = parent._extensions;

        }

        #endregion
    }
}
