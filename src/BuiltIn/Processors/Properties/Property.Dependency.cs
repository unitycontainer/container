using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor
    {
        protected override DependencyInfo<PropertyInfo> GetDependencyInfo(PropertyInfo member)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<PropertyInfo>(member, member.PropertyType)
                : new DependencyInfo<PropertyInfo>(member, import.ContractType ?? member.PropertyType,
                                                           import,
                                                           import.AllowDefault);
        }

        protected override DependencyInfo<PropertyInfo> GetDependencyInfo(PropertyInfo member, object? data)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<PropertyInfo>(member, member.PropertyType, data)
                : new DependencyInfo<PropertyInfo>(member, import.ContractType ?? member.PropertyType,
                                                           import, data,
                                                           import.AllowDefault);
        }
    }
}
