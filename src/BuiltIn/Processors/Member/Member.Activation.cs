using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Resolve(ref DependencyInfo dependency)
        {
            // Required or optional

            return dependency.Context.Container.Resolve(ref dependency.Contract, ref dependency.Context);
        }
    }
}
