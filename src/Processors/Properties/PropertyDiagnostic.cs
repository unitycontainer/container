using System;
using System.Collections.Generic;
using System.Text;
using Unity.Policy;

namespace Unity.Processors
{
    public class PropertyDiagnostic : PropertyProcessor
    {
        #region Constructors

        public PropertyDiagnostic(IPolicySet policySet) 
            : base(policySet)
        {
        }

        #endregion


    }
}
