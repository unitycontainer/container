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

            // Initialize injected members
            for (var injected = GetInjected(context.Registration); null != injected; injected = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injected.Next))
            {
                int position;

                using var injection = context.Start(injected);

                if (-1 == (position = injected.SelectFrom(members)))
                {
                    injection.Error($"Injected member '{injected}' doesn't match any {typeof(TDependency).Name} on type {type}");
                    return;
                }

                if (set[position]) continue;
                else set[position] = true;

                using var action = context.Start(members[position]);

                Activate(ref context, injected.Data);
            }

            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var import = GetImportAttribute(member);
                
                if (null == import) continue;

                if (set[index]) continue;
                else set[index] = true;

                using var action = context.Start(member);

                Activate(ref context, import);
            }
        }
    }
}
