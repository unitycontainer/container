using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, object>
    {
        public FieldProcessor(Defaults defaults)
        {
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
