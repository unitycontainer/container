using System;
using System.Collections.Generic;
using System.Text;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldDiagnostic : FieldProcessor
    {
        #region Constructors

        public FieldDiagnostic(IPolicySet policySet) : base(policySet)
        {
        }

        #endregion
    }
}
