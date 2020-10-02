using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public override object? Activate(ref DependencyContext<ParameterInfo> dependency)
        {
            var parameter = Unsafe.As<ParameterInfo>(dependency.Info);

            dependency.Import = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute), true);
            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType, 
                               dependency.Import.ContractName);

            var @override = dependency.GetOverride();

            return (null != @override) 
                ? dependency.GetValue(@override.Value)
                : base.Activate(ref dependency);
        }


        public override object? Activate(ref DependencyContext<ParameterInfo> dependency, object? data)
        {
            ParameterInfo parameter = Unsafe.As<ParameterInfo>(dependency.Info);
            ResolverOverride? @override;

            dependency.Import = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute), true);

            if (data is Type target && typeof(Type) != parameter.ParameterType)
            {
                dependency.Contract = (parameter.ParameterType != target)
                    ? new Contract(target)
                    : null == dependency.Import
                        ? new Contract(parameter.ParameterType)
                        : new Contract(dependency.Import.ContractType ?? parameter.ParameterType, 
                                       dependency.Import.ContractName);

                @override = dependency.GetOverride();

                return (null != @override) 
                    ? dependency.GetValue(@override.Value)
                    : base.Activate(ref dependency);
            }

            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType,
                               dependency.Import.ContractName);

            @override = dependency.GetOverride();

            return (null != @override)
                ? dependency.GetValue(@override.Value)
                : dependency.GetValue(data);
        }
    }
}
