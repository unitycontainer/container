using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, PropertyInfo, object>
    {
        #region Constructors

        public PropertyProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        protected override ImportAttribute? GetImportAttribute(PropertyInfo info) 
            => (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));

        protected override Type DependencyType(PropertyInfo info) => info.PropertyType;

        #endregion
    }
}
