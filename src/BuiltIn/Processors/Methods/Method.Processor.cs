using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override MethodInfo[] GetMembers(Type type) => type.GetMethods(BindingFlags);

        #endregion
    }
}
