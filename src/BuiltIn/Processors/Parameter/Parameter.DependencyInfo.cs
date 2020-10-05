using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        #region To Dependency

        protected override DependencyInfo<ParameterInfo> ToDependencyInfo(ParameterInfo member, ImportAttribute? attribute = null)
        {
            var import = attribute ?? (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<ParameterInfo>(member, member.ParameterType, member.HasDefaultValue)
                : new DependencyInfo<ParameterInfo>(member, import.ContractType ?? member.ParameterType, 
                                                            import, 
                                                            import.AllowDefault || member.HasDefaultValue);
        }

        protected override DependencyInfo<ParameterInfo> ToDependencyInfo(ParameterInfo member, object? data)
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

        #endregion


        #region To Dependency Array

        protected DependencyInfo<ParameterInfo>[]? ToDependencyArray(ParameterInfo[] parameters, object[] data)
        {
            if (0 == parameters.Length) return null;

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = ToDependencyInfo(parameters[i], data[i]);
            }

            return dependencies;
        }

        protected DependencyInfo<ParameterInfo>[]? ToDependencyArray(ParameterInfo[] parameters)
        {
            if (0 == parameters.Length) return null;

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = ToDependencyInfo(parameters[i]);
            }

            return dependencies;
        }
        
        #endregion
    }
}
