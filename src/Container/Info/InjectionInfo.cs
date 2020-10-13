using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public readonly struct InjectionInfo<TInfo>
    {
        #region Fields

        public readonly ImportData        Data;
        public readonly ImportInfo<TInfo> Import;

        #endregion


        #region Constructors

        public InjectionInfo(TInfo info, Type contractType, string? contractName, bool allowDefault)
        {
            Import = new ImportInfo<TInfo>(info, contractType, contractName, allowDefault);
            Data = default;
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            Data = new ImportData(data, import);
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault, object? data)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            Data = info.AsImportData(contractType, data);
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            Data = default;
        }

        #endregion
    }


    public static class InjectionInfoExtensions
    {
        public static InjectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo info, object? data)
        {
            return data switch
            {
                IInjectionInfoProvider<ParameterInfo> provider      
                    => provider.GetInfo(info),

                Type target when typeof(Type) != info.ParameterType 
                    => new InjectionInfo<ParameterInfo>(info, target, info.HasDefaultValue),

                IResolve iResolve                                   
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, (ResolveDelegate<PipelineContext>)iResolve.Resolve,           ImportType.Pipeline),

                ResolveDelegate<PipelineContext> resolver           
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, data,                                                         ImportType.Pipeline),

                IResolverFactory<ParameterInfo> infoFactory         
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, infoFactory.GetResolver<PipelineContext>(info),               ImportType.Pipeline),

                IResolverFactory<Type> typeFactory                  
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, typeFactory.GetResolver<PipelineContext>(info.ParameterType), ImportType.Pipeline),

                RegistrationManager.InvalidValue _                  
                    => GetContractedInfo(info),

                _ => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, data, ImportType.Value),
            };

            static InjectionInfo<ParameterInfo> GetContractedInfo(ParameterInfo info)
            {
                var attribute = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));
                return new InjectionInfo<ParameterInfo>(info, attribute?.ContractType ?? info.ParameterType, attribute?.ContractName, info.HasDefaultValue);
            }
        }
    }
}
