using System.ComponentModel.Composition;
using System.Reflection;

namespace Unity.Container
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            var attribute = info.MemberInfo.GetCustomAttribute<ImportAttribute>(true);

            info.Data.ImportType = ImportType.None;
            if (info.MemberInfo.HasDefaultValue)
            {
                info.AllowDefault = true;
                info.Default.Value = info.MemberInfo.DefaultValue;
                info.Default.ImportType = ImportType.Value;
            }
            else
            { 
                info.AllowDefault = false;
                info.Default.ImportType = ImportType.None;
            }

            if (null != attribute)
            {
                info.Contract = new Contract(attribute.ContractType ?? info.MemberInfo.ParameterType, attribute.ContractName);
                info.AllowDefault |= attribute.AllowDefault;
                info.Source = attribute.Source;
                info.Policy = attribute.RequiredCreationPolicy;

                return ImportType.Attribute;
            }

            info.Contract = new Contract(info.MemberInfo.ParameterType);
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return ImportType.None;
        }
    }
}

