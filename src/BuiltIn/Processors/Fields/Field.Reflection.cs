using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor 
    {
        private static bool DefaultReflectionProvider(ref ReflectionInfo<FieldInfo> info)
        {
            var attribute = info.Import.Member.GetCustomAttribute<ImportAttribute>(true);

            if (null != attribute)
            {
                info.Import.ContractType = attribute.ContractType ?? info.Import.Member.FieldType;
                info.Import.ContractName = attribute.ContractName;
                info.Import.AllowDefault = attribute.AllowDefault;
                info.Import.Source = attribute.Source;
                info.Import.Policy = attribute.RequiredCreationPolicy;
                info.Data = DefaultImportParser(ref info.Import, attribute);
                return true;
            }

            info.Import.ContractType = info.Import.Member.FieldType;
            info.Import.ContractName = null;
            info.Import.AllowDefault = false;
            info.Import.Source = ImportSource.Any;
            info.Import.Policy = CreationPolicy.Any;
            info.Data = default;

            return false;
        }

        private static ImportData DefaultImportParser(ref ImportInfo<FieldInfo> info, object? value)
        {
            switch (value)
            {
                case IReflectionProvider<FieldInfo> provider:
                    var data = provider.GetReflectionInfo(ref info);
                    return ImportType.Unknown == data.DataType
                        ? DefaultImportParser(ref info, data.Value)
                        : data;

                case IResolve iResolve:
                    return new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline);

                case IResolverFactory<FieldInfo> infoFactory:
                    return new ImportData(infoFactory.GetResolver<PipelineContext>(info.Member), ImportType.Pipeline);

                case IResolverFactory<Type> typeFactory:
                    return new ImportData(typeFactory.GetResolver<PipelineContext>(info.Member.FieldType), ImportType.Pipeline);

                default:
                    return default;
            }
        }


        private static ImportData DefaultDataParser(ref ImportInfo<FieldInfo> info, object? value)
        {
            ImportData data;

            switch (value)
            {
                case Type target when typeof(Type) != info.Member.FieldType:
                    info.ContractType = target;
                    return default;

                case IReflectionProvider<FieldInfo> provider:
                    data = provider.GetReflectionInfo(ref info);
                    break;

                case IResolverFactory<FieldInfo> infoFactory:
                    return new ImportData(infoFactory.GetResolver<PipelineContext>(info.Member), ImportType.Pipeline);

                default:
                    data = ImportData.ToImportData(info.Member.FieldType, value);
                    break;
            }

            while (ImportType.Unknown == data.DataType)
            {
                data = DefaultDataParser(ref info, data.Value);
            }

            return data;
        }
    }
}
