using System.ComponentModel.Composition;
using System.Reflection;

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
                info.Contract = new Contract(attribute.ContractType ?? info.MemberInfo.PropertyType, attribute.ContractName);
                info.AllowDefault = attribute.AllowDefault;
                info.Source       = attribute.Source;
                info.Policy       = attribute.RequiredCreationPolicy;

                return ImportType.Attribute;
            }

            info.Contract = new Contract(info.MemberInfo.PropertyType);
            info.AllowDefault = false;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return ImportType.None;
        }
    }
}
