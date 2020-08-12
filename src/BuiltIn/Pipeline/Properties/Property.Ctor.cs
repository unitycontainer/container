using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {
        public PropertyProcessor(Defaults defaults)
        {
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
