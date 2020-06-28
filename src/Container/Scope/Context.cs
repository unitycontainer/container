using System;
using System.Collections.Generic;

namespace Unity.Scope
{
    public partial class RegistrationScope
    {
        #region Disposables

        /// <inheritdoc />
        public ICollection<IDisposable> Lifetime { get; } 
            = new List<IDisposable>();

        #endregion
    }
}
