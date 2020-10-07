using System;
using System.ComponentModel.Composition;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        protected void SetValue(ref PipelineContext context, ref DependencyInfo<TDependency> dependency)
        {
            object? value;
            PipelineContext local;
            ResolverOverride? @override;

            using var action = context.Start(dependency.Info);

            if (dependency.AllowDefault)
            {
                ErrorInfo error = default;

                // Local context
                local = context.CreateContext(ref dependency.Contract, ref error);

                value = null != (@override = local.GetOverride(ref dependency))
                    ? local.GetValue(dependency.Info, @override.Value)
                    : local.Resolve(ref dependency);

                // Swallow error and move on
                if (local.IsFaulted) return;
            }
            else
            {
                // Local context
                local = context.CreateContext(ref dependency.Contract);

                value = null != (@override = local.GetOverride(ref dependency))
                    ? local.GetValue(dependency.Info, @override.Value)
                    : local.Resolve(ref dependency);
            }

            if (context.IsFaulted) return;

            SetValue(dependency.Info, context.Target!, value);
        }


        protected virtual DependencyInfo<TDependency> ToDependencyInfo(TDependency member)
        {
            var import = GetImportAttribute(member);
            return (null == import)
                ? new DependencyInfo<TDependency>(member, MemberType(member))
                : new DependencyInfo<TDependency>(member, import.ContractType ?? MemberType(member),
                                                          import,
                                                          import.AllowDefault);
        }

        protected virtual DependencyInfo<TDependency> ToDependencyInfo(TDependency member, ImportAttribute import)
        {
            return new DependencyInfo<TDependency>(member, import.ContractType ?? MemberType(member),
                                                           import,
                                                           import.AllowDefault);
        }

        protected virtual DependencyInfo<TDependency> ToDependencyInfo(TDependency member, object? data)
        {
            var import = GetImportAttribute(member);
            var type = MemberType(member);

            if (data is Type target && typeof(Type) != type)
            {
                return (type != target)
                    ? new DependencyInfo<TDependency>(member, target, import?.AllowDefault ?? false)
                    : null == import
                        ? new DependencyInfo<TDependency>(member, type, import?.AllowDefault ?? false)
                        : new DependencyInfo<TDependency>(member, import.ContractType ?? type,
                                                                    import,
                                                                    import?.AllowDefault ?? false);
            }

            return (null == import)
                ? new DependencyInfo<TDependency>(member, type, data)
                : new DependencyInfo<TDependency>(member, import.ContractType ?? type,
                                                          import, data,
                                                          import.AllowDefault);
        }

    }
}
