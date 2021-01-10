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

            policies.Set<ImportDescriptionProvider<ParameterInfo, ImportDescriptor<ParameterInfo>>>(DefaultParameterImportProvider);
            policies.Set<ImportDescriptionProvider<PropertyInfo,  ImportDescriptor<PropertyInfo>>>(DefaultPropertyImportProvider);
            policies.Set<ImportDescriptionProvider<FieldInfo,     ImportDescriptor<FieldInfo>>>(DefaultFieldImportProvider);
        }

        #endregion
    }
}
