using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public static class ReflectionInfoExtensions
    {
        public static ReflectionInfo<Type> AsInjectionInfo(this Type type, object? data)
        {
            return data switch
            {
                IReflectionProvider<Type> provider 
                    => provider.GetInfo(type),

                Type contractType when typeof(Type) != type 
                    => new ReflectionInfo<Type>(type, contractType, null, ImportType.None),

                IResolve iResolve
                    => new ReflectionInfo<Type>(type, type, (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),

                ResolveDelegate<PipelineContext> resolver
                    => new ReflectionInfo<Type>(type, type, data, ImportType.Pipeline),

                IResolverFactory<Type> typeFactory
                    => new ReflectionInfo<Type>(type, type, typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline),

                RegistrationManager.InvalidValue _
                    => new ReflectionInfo<Type>(type, type, null, ImportType.None),

                _ => new ReflectionInfo<Type>(type, type, data, ImportType.Value),
            };
        }

        public static ReflectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo parameter)
        {
            ImportAttribute? attribute = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute));

            return null == (attribute)
                ? new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, null, parameter.HasDefaultValue)
                : new ReflectionInfo<ParameterInfo>(parameter, attribute.ContractType ?? parameter.ParameterType,
                                                              attribute.ContractName,
                                                              attribute.AllowDefault || parameter.HasDefaultValue);
        }

        public static ReflectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo parameter, object? data)
        {
            return data switch
            {
                IReflectionProvider<ParameterInfo> provider
                    => provider.GetInfo(parameter),

                Type target when typeof(Type) != parameter.ParameterType
                    => new ReflectionInfo<ParameterInfo>(parameter, target, parameter.HasDefaultValue),

                IResolve iResolve
                    => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue, 
                                                        (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),

                ResolveDelegate<PipelineContext> resolver
                    => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue, 
                                                        data, ImportType.Pipeline),

                IResolverFactory<ParameterInfo> infoFactory
                    => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue, 
                                                        infoFactory.GetResolver<PipelineContext>(parameter), ImportType.Pipeline),

                IResolverFactory<Type> typeFactory
                    => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue, 
                                                        typeFactory.GetResolver<PipelineContext>(parameter.ParameterType), ImportType.Pipeline),

                RegistrationManager.InvalidValue _
                    => GetContractedInfo(parameter),

                _ => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue, 
                                                      data, ImportType.Value),
            };

            static ReflectionInfo<ParameterInfo> GetContractedInfo(ParameterInfo info)
            {
                var attribute = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));
                return new ReflectionInfo<ParameterInfo>(info, attribute?.ContractType ?? info.ParameterType, 
                                                              attribute?.ContractName, info.HasDefaultValue);
            }
        }
    }
}
