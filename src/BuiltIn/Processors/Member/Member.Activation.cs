using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            // Nothing to do if null
            if (null == context.Target) return;

            // Type to build
            Type type = context.Type;
            var members = GetMembers(type);

            // No members
            if (0 == members.Length) return;

            Span<bool> set = stackalloc bool[members.Length];
            DependencyInfo<TDependency> dependency;

            // Initialize injected members
            for (var injected = GetInjected(context.Registration); null != injected && !context.IsFaulted; 
                     injected = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injected.Next))
            {
                int index;
                using var action = context.Start(injected);

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    // TODO: Proper handling?
                    context.Error($"Injected member '{injected}' doesn't match any {typeof(TDependency).Name} on type {type}");
                    return;
                }

                if (set[index]) continue;
                else set[index] = true;

                var info = Unsafe.As<TDependency>(members[index]);
                dependency = ToDependencyInfo(info, injected.Data); 
                SetValue(ref context, ref dependency);
            }

            // Initialize annotated members
            for (var index = 0; index < members.Length && !context.IsFaulted; index++)
            {
                if (set[index]) continue;

                var info = Unsafe.As<TDependency>(members[index]);
                var import = GetImportAttribute(Unsafe.As<TMemberInfo>(info));

                if (null == import) continue;
                else set[index] = true;

                dependency = ToDependencyInfo(info, import);
                SetValue(ref context, ref dependency);
            }
        }
    }
}
