using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public override object? Activate(ref DependencyInfo<ParameterInfo> dependency, object? data)
        {
            ResolverOverride? @override;
            ParameterInfo parameter = dependency.Info;

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
                    ? base.Activate(ref dependency, @override.Value)
                    : dependency.Parent.Container.Resolve(ref dependency.Contract, ref dependency.Parent);
            }

            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType,
                               dependency.Import.ContractName);

            dependency.AllowDefault = dependency.Import?.AllowDefault ?? false || parameter.HasDefaultValue;

            @override = dependency.GetOverride();

            return (null != @override)
                ? base.Activate(ref dependency, @override.Value)
                : base.Activate(ref dependency, data);
        }

        public override object? Activate(ref DependencyInfo<ParameterInfo> dependency)
        {
            ParameterInfo parameter = dependency.Info;

            dependency.Import = (ImportAttribute?)parameter.GetCustomAttribute(typeof(ImportAttribute), true);
            dependency.Contract = (null == dependency.Import)
                ? new Contract(parameter.ParameterType)
                : new Contract(dependency.Import.ContractType ?? parameter.ParameterType,
                               dependency.Import.ContractName);
            dependency.AllowDefault = dependency.Import?.AllowDefault ?? false || parameter.HasDefaultValue;

            var @override = dependency.GetOverride();

            return (null != @override)
                ? base.Activate(ref dependency, @override.Value)
                : dependency.Parent.Container.Resolve(ref dependency.Contract, ref dependency.Parent);
        }
    }
}
