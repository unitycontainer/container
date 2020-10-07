using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            Type type = builder.Context.Type;
            var members = GetMembers(type);
            var downstream = builder.Build();

            // Check if any methods are available
            if (0 == members.Length) return downstream;

            int count = 0;
            Span<bool> set = stackalloc bool[members.Length];
            DependencyInfo<TDependency>[]? dependencies = null;

            // Initialize injected members
            for (var injected = GetInjected(builder.Context.Registration); null != injected; injected = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injected.Next))
            {
                int index;

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    // TODO: Proper handling?
                    builder.Context.Error($"Injected member '{injected}' doesn't match any {typeof(TDependency).Name} on type {type}");
                    return downstream;
                }

                if (set[index]) continue;
                else set[index] = true;
                
                (dependencies ??= new DependencyInfo<TDependency>[members.Length])[count++] = ToDependencyInfo(Unsafe.As<TDependency>(members[index]), injected.Data);
            }

            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var info = Unsafe.As<TDependency>(members[index]);
                var import = GetImportAttribute(info);

                if (null == import) continue;
                else set[index] = true;
                
                (dependencies ??= new DependencyInfo<TDependency>[members.Length])[count++] = ToDependencyInfo(Unsafe.As<TDependency>(members[index]), import);
            }

            // Validate and trim dependency array
            if (null == dependencies || 0 == count) return downstream;
            if (dependencies.Length > count) Array.Resize(ref dependencies, count);

            // Create pipeline
            return (ref PipelineContext context) =>
            {
                // Nothing to do if null
                if (null == context.Target) return context.Target;

                for (var index = 0; index < dependencies.Length; index++)
                {
                    ref var dependency = ref dependencies[index];
                    SetValue(ref context, ref dependency);
                }

                return downstream?.Invoke(ref context);
            };
        }
    }
}
