using System;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        //public delegate ImportData InjectionProcessor

        protected virtual ImportData FromInjected<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import, InjectionMember<TMemberInfo, TData> injection)
            where TContext : IBuilderContext
        {

            injection.DescribeImport(ref import);

            if (!import.RequireBuild) return import.ValueData;
                
            return Build(ref context, ref import, import.ValueData.Value);
        }
    }
}
