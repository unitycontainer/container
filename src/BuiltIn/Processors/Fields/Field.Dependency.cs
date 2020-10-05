using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor
    {
        protected override DependencyInfo<FieldInfo> GetDependencyInfo(FieldInfo member)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<FieldInfo>(member, member.FieldType)
                : new DependencyInfo<FieldInfo>(member, import.ContractType ?? member.FieldType,
                                                        import,
                                                        import.AllowDefault);
        }

        protected override DependencyInfo<FieldInfo> GetDependencyInfo(FieldInfo member, object? data)
        {
            var import = (ImportAttribute?)member.GetCustomAttribute(typeof(ImportAttribute), true);
            return (null == import)
                ? new DependencyInfo<FieldInfo>(member, member.FieldType, data)
                : new DependencyInfo<FieldInfo>(member, import.ContractType ?? member.FieldType,
                                                        import, data,
                                                        import.AllowDefault);
        }
    }
}
