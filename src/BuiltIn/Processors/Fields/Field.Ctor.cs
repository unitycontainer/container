using System;
using System.Reflection;
using Unity.Container;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberInfoProcessor<FieldInfo, object>
    {
        #region Constructors

        public FieldProcessor(Defaults defaults)
            : base(defaults)
        {
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        #endregion


        #region Policy Changes

        private void OnDefaultsChanged(Type type, object? value)
        {
        }

        #endregion


        #region Implementation

        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(SupportedBindingFlags);
        

        #endregion
    }
}
