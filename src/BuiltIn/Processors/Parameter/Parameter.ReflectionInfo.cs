using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        private static ReflectionInfo<ParameterInfo> ToInjectionInfo(ParameterInfo parameter)
        {
            ImportAttribute? attribute = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute));

            return null == (attribute)
                ? new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, null, parameter.HasDefaultValue)
                : new ReflectionInfo<ParameterInfo>(parameter, attribute.ContractType ?? parameter.ParameterType,
                                                              attribute.ContractName,
                                                              attribute.AllowDefault || parameter.HasDefaultValue);
        }

        private static ReflectionInfo<ParameterInfo> ToInjectionInfoFromData(ParameterInfo parameter, object? data)
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

                RegistrationManager.InvalidValue _ => ToInjectionInfo(parameter),

                _ => new ReflectionInfo<ParameterInfo>(parameter, parameter.ParameterType, parameter.HasDefaultValue,
                                                      data, ImportType.Value),
            };

        }
    }
}

