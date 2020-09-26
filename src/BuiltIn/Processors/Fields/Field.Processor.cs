using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, object>
    {
        #region Constructors

        public FieldProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(BindingFlags);


        #endregion



        #region Overrides

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        #endregion


    }
}
