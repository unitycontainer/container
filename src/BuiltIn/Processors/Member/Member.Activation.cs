using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Build(ref PipelineContext context, TDependency info, object? data)
        {
            throw new NotImplementedException();
        }

        public virtual object? Build(ref PipelineContext context, TDependency info)
        {
            // Get annotation info
            var attribute = GetImportAttribute(info);

            // Get contract of the dependency
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            var local = context.Create(ref contract, info);

            ResolverOverride[] overrides;

            if ((null != (overrides = context.Overrides)) && TryGetOverride(ref local, overrides))
            {
            }


            throw new NotImplementedException();
        }
    }
}
