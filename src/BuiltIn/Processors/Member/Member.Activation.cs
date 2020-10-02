using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Activate(ref DependencyContext<TDependency> dependency, object? data)
        {
            ResolverOverride? @override;

            @override = dependency.GetOverride();

            return (null != @override)
                ? dependency.GetValue(@override.Value)
                : dependency.GetValue(data);
        }

        public virtual object? Activate(ref DependencyContext<TDependency> dependency)
        {
            // Required or optional

            return dependency.Parent.Container.Resolve(ref dependency.Contract, ref dependency.Parent);
        }

        protected abstract void Activate(ref PipelineContext context, TData data);

        protected virtual void Activate(ref PipelineContext context, ImportAttribute import) => throw new NotImplementedException();
    }
}
