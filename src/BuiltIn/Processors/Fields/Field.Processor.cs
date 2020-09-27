using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        public FieldProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion

        protected override Type MemberType(FieldInfo info) => info.FieldType;


        #region Implementation

        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(BindingFlags);

        protected override ImportAttribute? GetImportAttribute(FieldInfo info) 
            => (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));

        protected override Type DependencyType(FieldInfo info) => info.FieldType;

        #endregion


    }
}
