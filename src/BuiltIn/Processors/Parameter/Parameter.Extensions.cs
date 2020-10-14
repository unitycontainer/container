using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class ParameterProcessorExtensions
    {
        public static ParameterProcessor<TMemberInfo>.InvokeInfo AsInvokeInfo<TMemberInfo>(this TMemberInfo info)
            where TMemberInfo : MethodBase
        {
            var parameters = info.GetParameters();
            var arguments = new InjectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = parameters[i].AsInjectionInfo();

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        public static ParameterProcessor<TMemberInfo>.InvokeInfo AsInvokeInfo<TMemberInfo>(this TMemberInfo info, object?[]? data)
            where TMemberInfo : MethodBase
        {
            var parameters = info.GetParameters();
            var arguments = new InjectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = parameters[i].AsInjectionInfo(data![i]);

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        public static InjectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo info)
        {
            ImportAttribute? attribute = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));

            return null == (attribute)
                ? new InjectionInfo<ParameterInfo>(info, info.ParameterType, null, info.HasDefaultValue)
                : new InjectionInfo<ParameterInfo>(info, attribute.ContractType ?? info.ParameterType,
                                                         attribute.ContractName,
                                                         attribute.AllowDefault || info.HasDefaultValue);
        }


        //public static InjectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo info)
        //{
        //    ImportAttribute? attribute = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute));

        //    return null == (attribute)
        //        ? new InjectionInfo<ParameterInfo>(info, info.ParameterType, null, info.HasDefaultValue)
        //        : (attribute.AllowDefault && !info.HasDefaultValue)
        //            ? new InjectionInfo<ParameterInfo>(info, attribute.ContractType ?? info.ParameterType,
        //                                                     attribute.ContractName, true, GetDefaultValue(info.ParameterType), ImportType.Default)
        //            : new InjectionInfo<ParameterInfo>(info, attribute.ContractType ?? info.ParameterType,
        //                                                     attribute.ContractName,
        //                                                     attribute.AllowDefault || info.HasDefaultValue);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public static InjectionInfo<ParameterInfo> AsInjectionInfo(this ParameterInfo info, object? data)
        {
            return data switch
            {
                IInjectionInfoProvider<ParameterInfo> provider
                    => provider.GetInfo(info),

                Type target when typeof(Type) != info.ParameterType
                    => new InjectionInfo<ParameterInfo>(info, target, info.HasDefaultValue),

                IResolve iResolve
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),

                ResolveDelegate<PipelineContext> resolver
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, data, ImportType.Pipeline),

                IResolverFactory<ParameterInfo> infoFactory
                    => new InjectionInfo<ParameterInfo>(info, info.ParameterType, info.HasDefaultValue, infoFactory.GetResolver<PipelineContext>(info), ImportType.Pipeline),

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

