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
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        #endregion

        #region Policy Changes

        private void OnDefaultsChanged(Type type, object? value)
        {
        }

        #endregion


        #region Implementation

        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(SupportedBindingFlags);

        #endregion
    }
}
