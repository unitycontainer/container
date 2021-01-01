using System;
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
            policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(DefaultConstructorSelector);

            
            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            // These selectors are used by Build strategies to get declared members

            policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(GetConstructors);
            policies.Set<PropertyInfo, MembersSelector<PropertyInfo>>(GetProperties);
            policies.Set<MethodInfo, MembersSelector<MethodInfo>>(GetMethods);
            policies.Set<FieldInfo, MembersSelector<FieldInfo>>(GetFields);

            
            // Array target type selector
            policies.Set<Array, SelectorDelegate<Type, Type>>(ArrayTargetTypeSelector);
        }
    }
}
