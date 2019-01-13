using System;
using System.Collections.Generic;
using System.Text;
using Unity.Policy;

namespace Unity.Processors
{
    public class MethodDiagnostic : MethodProcessor
    {
        #region Constructors

        public MethodDiagnostic(IPolicySet policySet) 
            : base(policySet)
        {
        }

        #endregion
    }
}
