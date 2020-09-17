using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberInfoProcessor<FieldInfo, object>
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
    }
}
