using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion
    }
}
