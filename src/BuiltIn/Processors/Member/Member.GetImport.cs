using System;
using System.ComponentModel.Composition;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        protected virtual void Build(ref PipelineContext context)
        {
            //object? value;
            //PipelineContext local;
            //ResolverOverride? @override;

            //if (dependency.AllowDefault)
            //{
            //    ErrorInfo error = default;

            //    // Local context
            //    local = context.CreateContext(ref dependency.Contract, ref error);

            //    value = null != (@override = local.GetOverride(ref dependency))
            //        ? local.GetValue(dependency.Info, @override.Value)s
            //        : local.Resolve(ref dependency);

            //    // Swallow error and move on
            //    if (local.IsFaulted) return;
            //}
            //else
            //{
            //    // Local context
            //    local = context.CreateContext(ref dependency.Contract);

            //    value = null != (@override = local.GetOverride(ref dependency))
            //        ? local.GetValue(dependency.Info, @override.Value)
            //        : local.Resolve(ref dependency);
            //}

            //if (context.IsFaulted) return;

            //SetValue(dependency.Info, context.Target!, value);
        }
    }
}
