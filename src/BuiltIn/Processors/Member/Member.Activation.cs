using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target);

            // Type to build
            Type type = context.Type;
            var members = GetMembers(type);

            ///////////////////////////////////////////////////////////////////
            // No members
            if (0 == members.Length) return;

            Span<bool> set = stackalloc bool[members.Length];
            var dependency = new MemberDependency(ref context);

            ///////////////////////////////////////////////////////////////////
            // Initialize injected members
            for (var injected = GetInjected(context.Registration); null != injected; injected = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injected.Next))
            {
                int index;

                using var injection = context.Start(injected);

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    injection.Error($"Injected member '{injected}' doesn't match any {typeof(TDependency).Name} on type {type}");
                    return;
                }

                if (set[index]) continue;
                else set[index] = true;

                dependency.Info   = Unsafe.As<TDependency>(members[index]);
                dependency.Import = GetImportAttribute(Unsafe.As<TMemberInfo>(dependency.Info));

                Activate(ref dependency, injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                dependency.Info   = Unsafe.As<TDependency>(members[index]);
                dependency.Import = GetImportAttribute(Unsafe.As<TMemberInfo>(dependency.Info));
                
                if (null == dependency.Import) continue;

                if (set[index]) continue;
                else set[index] = true;


                //using var action = context.Start(member);

                Activate(ref dependency);
            }
        }

        public abstract object? Activate(ref MemberDependency dependency, object? data);

        public abstract object? Activate(ref MemberDependency dependency);
    }
}
