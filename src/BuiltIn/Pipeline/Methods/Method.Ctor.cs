using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : MethodBaseProcessor<MethodInfo>
    {
        public MethodProcessor(Defaults defaults)
        {
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
