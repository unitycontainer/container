using System.Reflection;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Selectors
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = context.Policies;

            // Set Constructor selector
            policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(ConstructorSelector);

            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(GetConstructors);
            policies.Set<PropertyInfo, MembersSelector<PropertyInfo>>(GetProperties);
            policies.Set<MethodInfo, MembersSelector<MethodInfo>>(GetMethods);
            policies.Set<FieldInfo, MembersSelector<FieldInfo>>(GetFields);
        }

    }
}
