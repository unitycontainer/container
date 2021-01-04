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

            //policies.Set<ImportProvider<ImportInfo, ImportType>>(typeof(ParameterInfo), DefaultImportProvider);
            policies.Set<ImportDescriptionProvider<ParameterInfo, MemberStrategy<ConstructorInfo, ParameterInfo, object[]>.ImportInfo>>(DefaultParameterImportProvider);
            policies.Set<ImportDescriptionProvider<ParameterInfo, MemberStrategy<MethodInfo,      ParameterInfo, object[]>.ImportInfo>>(DefaultParameterImportProvider);

            policies.Set<ImportDescriptionProvider<FieldInfo, MemberStrategy<FieldInfo, FieldInfo, object>.ImportInfo>>(DefaultFieldImportProvider);
            policies.Set<ImportDescriptionProvider<PropertyInfo, MemberStrategy<PropertyInfo, PropertyInfo, object>.ImportInfo>>(DefaultPropertyImportProvider);
        }

        #endregion
    }
}
