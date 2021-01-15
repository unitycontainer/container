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

            policies.Set<ImportDescriptionProvider<ConstructorInfo, MemberDescriptor<ConstructorInfo>>>(DefaultConstructorImportProvider);
            policies.Set<ImportDescriptionProvider<MethodInfo,      MemberDescriptor<MethodInfo>>>(DefaultMethodImportProvider);
            policies.Set<ImportDescriptionProvider<ParameterInfo,   MemberDescriptor<ParameterInfo>>>(DefaultParameterImportProvider);
            policies.Set<ImportDescriptionProvider<PropertyInfo,    MemberDescriptor<PropertyInfo>>>(DefaultPropertyImportProvider);
            policies.Set<ImportDescriptionProvider<FieldInfo,       MemberDescriptor<FieldInfo>>>(DefaultFieldImportProvider);
        }

        #endregion
    }
}
