using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Scope
{
    public partial class RegistrationScopeAsync : RegistrationScope
    {
        #region Constructors

        internal RegistrationScopeAsync() 
        { 
        }

        protected RegistrationScopeAsync(RegistrationScope parent)
            : base(parent) 
        {
        }

        #endregion


        #region Implementation

        public override RegistrationScope CreateChildScope()
            => new RegistrationScopeAsync(this);

        #endregion
    }
}
