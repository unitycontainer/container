using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial class FieldProcessor
    {

        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            var @default = info.MemberInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (@default is not null)
            {
                info.AllowDefault = true;
                info.Default.Value = @default.Value;
                info.Default.ImportType = ImportType.Value;
            }
            else
            {
                info.Default.ImportType = ImportType.None;
            }

            var attribute = info.MemberInfo.GetCustomAttribute<ImportAttribute>(true);
            info.Data.ImportType = ImportType.None;

            if (null != attribute)
            {
                info.Contract = new Contract(attribute.ContractType ?? info.MemberInfo.FieldType, attribute.ContractName);
                info.AllowDefault |= attribute.AllowDefault;
                info.Source = attribute.Source;
                info.Policy = attribute.RequiredCreationPolicy;

                return ImportType.Attribute;
            }

            info.Contract = new Contract(info.MemberInfo.FieldType);
            info.AllowDefault = false;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return ImportType.None;
        }

        private static ImportType Default_ImportProvider(ref ImportInfo info)
        {
            info.Attributes = Unsafe.As<Attribute[]>(info.MemberInfo.GetCustomAttributes(true));
            info.Data.ImportType = ImportType.None;
            info.Default.ImportType = ImportType.None;

            if (0 == info.Attributes.Length)
            {
                info.Contract = new Contract(info.MemberInfo.FieldType);
                info.Policy = CreationPolicy.Any;
                info.Source = ImportSource.Any;
                info.AllowDefault = false;

                return ImportType.None;
            }

            foreach (var attribute in info.Attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        info.Contract = new Contract(import.ContractType ?? info.MemberInfo.FieldType,
                                                     import.ContractName);
                        info.Policy = import.RequiredCreationPolicy;
                        info.Source = import.Source;
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
                        info.AllowDefault = true;
                        info.Default.Value = @default.Value;
                        info.Default.ImportType = ImportType.Value;
                        break;
                }
            }

            return ImportType.Attribute;
        }
    }
}


