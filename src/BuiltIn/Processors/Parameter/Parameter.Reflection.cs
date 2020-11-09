using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        private static ImportType DefaultImportProvider(ref ImportInfo<ParameterInfo> info)
        {
            var attribute = info.Member.GetCustomAttribute<ImportAttribute>(true);

            if (info.Member.HasDefaultValue)
            { 
                info.Data.Value      = info.Member.DefaultValue;
                info.Data.ImportType = ImportType.Default;
            }

            if (null != attribute)
            {
                info.ContractType  = attribute.ContractType ?? info.Member.ParameterType;
                info.ContractName  = attribute.ContractName;
                info.AllowDefault |= attribute.AllowDefault;
                info.Source        = attribute.Source;
                info.Policy        = attribute.RequiredCreationPolicy;

                return ImportType.Attribute;
            }

            info.ContractType = info.Member.ParameterType;
            info.ContractName = null;

            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;
            info.Data = default;

            return ImportType.None;
        }

        private static ImportType DefaultImportParser(ref ImportInfo<ParameterInfo> info)
        {
            while (ImportType.Unknown == info.Data.ImportType)
            {
                switch (info.Data.Value)
                {
                    case Type target when typeof(Type) != info.Member.ParameterType:
                        info.ContractType = target;
                        info.Data = default;
                        return ImportType.None;

                    case IImportProvider provider:
                        info.Data = default;
                        provider.GetImportInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.ImportType = ImportType.Pipeline;
                        info.ImportValue = (ResolveDelegate<PipelineContext>)iResolve.Resolve;
                        return ImportType.Pipeline;

                    case ResolveDelegate<PipelineContext> resolver:
                        info.ImportType = ImportType.Pipeline;
                        info.ImportValue = resolver;
                        return ImportType.Pipeline;

                    case IResolverFactory<Type> typeFactory:
                        info.ImportType = ImportType.Pipeline;
                        info.ImportValue = typeFactory.GetResolver<PipelineContext>(info.Member.ParameterType);
                        return ImportType.Pipeline;

                    case PipelineFactory factory:
                        info.ImportType = ImportType.Pipeline;
                        info.ImportValue = factory(info.Member.ParameterType);
                        return ImportType.Pipeline;

                    case IResolverFactory<ParameterInfo> infoFactory:
                        info.Data.Value = infoFactory.GetResolver<PipelineContext>(info.Member);
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case RegistrationManager.InvalidValue _:
                        info.Data = default;
                        return ImportType.None;

                    default:
                        info.ImportType = ImportType.Value;
                        return ImportType.Value;
                }
            }

            return info.ImportType;
        }
    }
}

