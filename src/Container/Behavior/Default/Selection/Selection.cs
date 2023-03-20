using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Selection
    {
        #region Setup

        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Default Constructor selection algorithm. This method determines which
            // Constructor is selected when there are multiple choices and noting is annotated.
            policies.Set<ConstructorSelector>(SelectConstructor);

            
            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            policies.Set<DeclaredMembers<ConstructorInfo>>(GetConstructors);
            policies.Set<DeclaredMembers<PropertyInfo>>(GetProperties);
            policies.Set<DeclaredMembers<MethodInfo>>(GetMethods);
            policies.Set<DeclaredMembers<FieldInfo>>(GetFields);
        }

        #endregion
    }
}
