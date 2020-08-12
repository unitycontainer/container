using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        public ConstructorProcessor(Defaults defaults)
        {
            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
