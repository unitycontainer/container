using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {

        private static bool DefaultReflectionProvider(ref ImportInfo<ParameterInfo> info)
        {
            var attribute = info.Member.GetCustomAttribute<ImportAttribute>(true);

            if (null != attribute)
            {
                info.ContractType = attribute.ContractType ?? info.Member.ParameterType;
                info.ContractName = attribute.ContractName;
                info.AllowDefault = attribute.AllowDefault || info.Member.HasDefaultValue;
                info.Source = attribute.Source;
                info.Policy = attribute.RequiredCreationPolicy;
                return true;
            }

            info.ContractType = info.Member.ParameterType;
            info.ContractName = null;
            info.AllowDefault = false;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;

            return false;
        }

        private static ImportData DefaultDataParser(ref ImportInfo<ParameterInfo> info, object? value)
        {
            switch (value)
            {
                case Type target when typeof(Type) != info.Member.ParameterType:
                    info.ContractType = target;
                    return default;

                case RegistrationManager.InvalidValue _:
                    return default;

                case IReflectionProvider<ParameterInfo> provider:
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

                case IResolverFactory<ParameterInfo> infoFactory:
                    return new ImportData(infoFactory.GetResolver<PipelineContext>(info.Member), ImportType.Pipeline);

                case IResolverFactory<Type> typeFactory:
                    return new ImportData(typeFactory.GetResolver<PipelineContext>(info.Member.ParameterType), ImportType.Pipeline);

                default:
                    return new ImportData(value, ImportType.Value);
            }
        }
    }
}

