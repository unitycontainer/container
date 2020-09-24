using System;
using System.Collections.Generic;
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


        #region Overrides

        protected override Type MemberType(PropertyInfo info) => info.PropertyType;

        protected override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
        {
            throw new NotImplementedException();
            //return type.SupportedProperties();
        }

        #endregion
    }
}
