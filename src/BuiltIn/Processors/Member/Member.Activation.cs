using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Build(ref PipelineContext context, TDependency info, object? data)
        {
            using var action = context.Start(info);

            var attribute = GetImportAttribute(info);
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            // Check if overriden
            var @override = context.GetOverride(info, in contract);
            var value = null == @override ? data : @override.Value;

            // Get value
            return context.Resolve(ref contract, info, value);
        }

        public virtual object? Build(ref PipelineContext context, TDependency info)
        {
            using var action = context.Start(info);

            var attribute = GetImportAttribute(info);
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            // Check if overriden
            var @override = context.GetOverride(info, in contract);
            var value = null == @override ? attribute : @override.Value;

            // Get value 
            return context.Resolve(ref contract, info, value);
        }
    }
}
