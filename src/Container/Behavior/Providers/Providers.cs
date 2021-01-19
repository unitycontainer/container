using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Providers
    {
        #region Setup

        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            policies.Set<ImportProvider<ConstructorInfo, MemberDescriptor<ConstructorInfo>>>(DefaultConstructorImportProvider);
            policies.Set<ImportProvider<MethodInfo,      MemberDescriptor<MethodInfo>>>(DefaultMethodImportProvider);
            policies.Set<ImportProvider<ParameterInfo,   MemberDescriptor<ParameterInfo>>>(DefaultParameterImportProvider);
            policies.Set<ImportProvider<PropertyInfo,    MemberDescriptor<PropertyInfo>>>(DefaultPropertyImportProvider);
            policies.Set<ImportProvider<FieldInfo,       MemberDescriptor<FieldInfo>>>(DefaultFieldImportProvider);
        }

        #endregion
    }
}
