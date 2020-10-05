using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor
    {
        public override object? Activate(ref DependencyInfo<PropertyInfo> dependency, object? data)
        {
            throw new System.NotImplementedException();
            //dependency.Contract = (null == dependency.Import)
            //    ? new Contract(dependency.Info.PropertyType)
            //    : new Contract(dependency.Import.ContractType ?? dependency.Info.PropertyType,
            //                   dependency.Import.ContractName);

            //var @override = dependency.GetOverride();

            //var value = (null != @override)
            //    ? base.Activate(ref dependency, @override.Value)
            //    : base.Activate(ref dependency, data);

            //if (!dependency.Parent.IsFaulted) dependency.Info.SetValue(dependency.Parent.Target, value);

            //return value;
        }

        public override object? Activate(ref DependencyInfo<PropertyInfo> dependency)
        {
            throw new System.NotImplementedException();
            //dependency.Contract = (null == dependency.Import)
            //    ? new Contract(dependency.Info.PropertyType)
            //    : new Contract(dependency.Import.ContractType ?? dependency.Info.PropertyType,
            //                   dependency.Import.ContractName);

            //var value = dependency.Parent
            //                      .Container
            //                      .Resolve(ref dependency.Contract, ref dependency.Parent);

            //if (!dependency.Parent.IsFaulted)
            //{ 
            //    dependency.Info.SetValue(dependency.Parent.Target, value);
            //}

            //return value;
        }
    }
}
