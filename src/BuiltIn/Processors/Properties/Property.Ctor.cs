using System;
using System.Reflection;
using Unity.Container;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberInfoProcessor<PropertyInfo, object>
    {
        #region Constructors

        public PropertyProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        #endregion
    }
}
