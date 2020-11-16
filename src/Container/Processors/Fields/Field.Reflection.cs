using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public partial class FieldProcessor 
    {
        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            var attributes = info.MemberInfo.GetCustomAttributes(true);

            info.Data.ImportType = ImportType.None;
            info.Default.ImportType = ImportType.None;
            
            if (0 == attributes.Length)
            {
                info.Contract = new Contract(info.MemberInfo.FieldType);
                info.Policy = CreationPolicy.Any;
                info.Source = ImportSource.Any;
                info.AllowDefault = false;

                return ImportType.None;
            }

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        info.Contract = new Contract(import.ContractType ?? info.MemberInfo.FieldType, 
                                                     import.ContractName);
                        info.Policy        = import.RequiredCreationPolicy;
                        info.Source        = import.Source;
                        info.AllowDefault |= import.AllowDefault;
                        break;

                    case ImportManyAttribute many:
                        info.Contract = new Contract(many.ContractType ?? info.MemberInfo.FieldType,
                                                     many.ContractName);
                        info.Policy = many.RequiredCreationPolicy;
                        info.Source = many.Source;
                        info.AllowDefault = false;
                        break;

                    case DefaultValueAttribute @default:
                        info.AllowDefault  = true;
                        info.Default.Value = @default.Value;
                        info.Default.ImportType = ImportType.Value;
                        break;
                }
            }
            
            return ImportType.Attribute;
        }
    }
}
