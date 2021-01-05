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

            ImportInfo<FieldInfo>.GetMemberType = (FieldInfo info) => info.FieldType;
            ImportInfo<FieldInfo>.GetDeclaringType = (FieldInfo info) => info.DeclaringType!;
            
            ImportInfo<PropertyInfo>.GetMemberType = (PropertyInfo info) => info.PropertyType;
            ImportInfo<PropertyInfo>.GetDeclaringType = (PropertyInfo info) => info.DeclaringType!;
            
            ImportInfo<ParameterInfo>.GetMemberType = (ParameterInfo info) => info.ParameterType;
            ImportInfo<ParameterInfo>.GetDeclaringType = (ParameterInfo info) => info.Member.DeclaringType!;


            policies.Set<ImportDescriptionProvider<ParameterInfo, ImportInfo<ParameterInfo>>>(DefaultParameterImportProvider);
            policies.Set<ImportDescriptionProvider<PropertyInfo,  ImportInfo<PropertyInfo>>>(DefaultPropertyImportProvider);
            policies.Set<ImportDescriptionProvider<FieldInfo,     ImportInfo<FieldInfo>>>(DefaultFieldImportProvider);
        }

        #endregion
    }
}
