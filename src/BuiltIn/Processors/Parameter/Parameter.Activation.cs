using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public override object? Resolve(ref DependencyInfo dependency)
        {
            var parameter = Unsafe.As<ParameterInfo>(dependency.Info);

            dependency.Import = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute), true);
            dependency.DeclaringType = parameter.Member.DeclaringType;
            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType, 
                               dependency.Import.ContractName);

            var @override = dependency.GetOverride<ParameterInfo>();

            return (null != @override) 
                ? dependency.GetValue<ParameterInfo>(@override.Value)
                : base.Resolve(ref dependency);
        }


        public override object? GetValue(ref DependencyInfo dependency, object? data)
        {
            ParameterInfo parameter = Unsafe.As<ParameterInfo>(dependency.Info);
            ResolverOverride? @override;

            dependency.Import = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute), true);
            dependency.DeclaringType = parameter.Member.DeclaringType;

            if (data is Type target && typeof(Type) != parameter.ParameterType)
            {
                dependency.Contract = (parameter.ParameterType != target)
                    ? new Contract(target)
                    : null == dependency.Import
                        ? new Contract(parameter.ParameterType)
                        : new Contract(dependency.Import.ContractType ?? parameter.ParameterType, 
                                       dependency.Import.ContractName);

                @override = dependency.GetOverride<ParameterInfo>();

                return (null != @override) 
                    ? dependency.GetValue<ParameterInfo>(@override.Value)
                    : base.Resolve(ref dependency);
            }

            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType,
                               dependency.Import.ContractName);

            @override = dependency.GetOverride<ParameterInfo>();

            return (null != @override)
                ? dependency.GetValue<ParameterInfo>(@override.Value)
                : dependency.GetValue<ParameterInfo>(data);
        }
    }
}
