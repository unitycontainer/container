using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor 
    {
        private static bool DefaultReflectionProvider(ref ImportInfo<FieldInfo> info)
        {
            var attribute = info.Member.GetCustomAttribute<ImportAttribute>(true);

            if (null != attribute)
            {
                info.ContractType = attribute.ContractType ?? info.Member.FieldType;
                info.ContractName = attribute.ContractName;
                info.AllowDefault = attribute.AllowDefault;
                info.Source = attribute.Source;
                info.Policy = attribute.RequiredCreationPolicy;
                return true;
            }

            info.ContractType = info.Member.FieldType;
            info.ContractName = null;
            info.AllowDefault = false;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return false;
        }

        private static ImportData DefaultDataParser(ref ImportInfo<FieldInfo> info, object? value)
        {
            switch (value)
            {
                case Type target when typeof(Type) != info.Member.FieldType:
                    info.ContractType = target;
                    return default;

                case RegistrationManager.InvalidValue _:
                    return default;

                case IReflectionProvider<FieldInfo> provider:
                    var data = provider.GetReflectionInfo(ref info);
                    return ImportType.Unknown == data.DataType
                        ? DefaultDataParser(ref info, data.Value)
                        : data;

                case IResolve iResolve:
                    return new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline);

                case PipelineFactory factory:
                    return new ImportData(factory, ImportType.Pipeline);

                case ResolveDelegate<PipelineContext> resolver:
                    return new ImportData(resolver, ImportType.Pipeline);

                case IResolverFactory<FieldInfo> infoFactory:
                    return new ImportData(infoFactory.GetResolver<PipelineContext>(info.Member), ImportType.Pipeline);

                case IResolverFactory<Type> typeFactory:
                    return new ImportData(typeFactory.GetResolver<PipelineContext>(info.Member.FieldType), ImportType.Pipeline);

                default:
                    return new ImportData(value, ImportType.Value);
            }
        }
    }
}
