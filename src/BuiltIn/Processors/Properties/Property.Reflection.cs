using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor
    {
        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            var attribute = info.MemberInfo.GetCustomAttribute<ImportAttribute>(true);

            info.Data.ImportType = ImportType.None;

            if (null != attribute)
            {
                info.ContractType = attribute.ContractType ?? info.MemberInfo.PropertyType;
                info.ContractName = attribute.ContractName;
                info.AllowDefault = attribute.AllowDefault;
                info.Source       = attribute.Source;
                info.Policy       = attribute.RequiredCreationPolicy;

                return ImportType.Attribute;
            }

            info.ContractType = info.MemberInfo.PropertyType;
            info.ContractName = null;
            info.AllowDefault = false;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return ImportType.None;
        }
    }
}
