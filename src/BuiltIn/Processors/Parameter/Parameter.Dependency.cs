using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected override DependencyInfo<ParameterInfo> GetDependencyInfo(ParameterInfo member)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<ParameterInfo>(member, member.ParameterType, member.HasDefaultValue)
                : new DependencyInfo<ParameterInfo>(member, import.ContractType ?? member.ParameterType, 
                                                            import, 
                                                            import.AllowDefault || member.HasDefaultValue);
        }

        protected override DependencyInfo<ParameterInfo> GetDependencyInfo(ParameterInfo member, object? data)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);

            if (data is Type target && typeof(Type) != member.ParameterType)
            {
                return (member.ParameterType != target)
                    ? new DependencyInfo<ParameterInfo>(member, target, import?.AllowDefault ?? false || member.HasDefaultValue)
                    : null == import
                        ? new DependencyInfo<ParameterInfo>(member, member.ParameterType, import?.AllowDefault ?? false || member.HasDefaultValue)
                        : new DependencyInfo<ParameterInfo>(member, import.ContractType ?? member.ParameterType,
                                                                    import, 
                                                                    import?.AllowDefault ?? false || member.HasDefaultValue);
            }

            return (null == import)
                ? new DependencyInfo<ParameterInfo>(member, member.ParameterType, data, member.HasDefaultValue)
                : new DependencyInfo<ParameterInfo>(member, import.ContractType ?? member.ParameterType, 
                                                            import, data,
                                                            import.AllowDefault || member.HasDefaultValue);
        }
    }
}
