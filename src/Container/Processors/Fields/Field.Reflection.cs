using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public partial class FieldProcessor
    {
        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            // Basics
            string? name = null;
            Type type = info.MemberInfo.FieldType;
            var result = ImportType.None;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;
            info.Data.ImportType = ImportType.None;
            info.AllowDefault = false;
            info.Default.ImportType = ImportType.None;

            // Process Attributes
            info.Attributes = Unsafe.As<Attribute[]>(info.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in info.Attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        if (import.ContractType is not null) type = import.ContractType;
                        name = import.ContractName;
                        info.Policy = import.RequiredCreationPolicy;
                        info.Source = import.Source;
                        info.AllowDefault |= import.AllowDefault;
                        info.Data.ImportType = ImportType.None;
                        result = ImportType.Attribute;
                        break;

                    case ImportManyAttribute many:
                        if (many.ContractType is not null) type = many.ContractType;
                        name = many.ContractName;
                        info.Policy = many.RequiredCreationPolicy;
                        info.Source = many.Source;
                        info.Data.ImportType = ImportType.None;
                        result = ImportType.Attribute;
                        break;

                    case DefaultValueAttribute @default:
                        info.AllowDefault = true;
                        info.Default.Value = @default.Value;
                        info.Default.ImportType = ImportType.Value;
                        if (ImportType.None == result)
                        {
                            info.Data.Value = @default.Value;
                            info.Data.ImportType = ImportType.Value;
                        }
                        result = ImportType.Attribute;
                        break;
                }
            }

            info.Contract = new Contract(type, name);

            return result;
        }
    }
}


