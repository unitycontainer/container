using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, object>
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
