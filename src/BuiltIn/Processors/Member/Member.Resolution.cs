using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        #region PipelineBuilder

        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            Type type = builder.Context.Type;
            var members = GetMembers(type);
            var downstream = builder.Build();

            ///////////////////////////////////////////////////////////////////
            // Check if any methods are available
            if (0 == members.Length) return downstream;

            int count = 0;
            Span<bool> set = stackalloc bool[members.Length];
            var dependencies = new DependencyInfo<TDependency>[members.Length];


            ///////////////////////////////////////////////////////////////////
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
                
                set[index] = true;
                dependencies[count++] = GetDependencyInfo(Unsafe.As<TDependency>(members[index]), injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var info = Unsafe.As<TDependency>(members[index]);
                var import = GetImportAttribute(Unsafe.As<TMemberInfo>(info));

                if (null == import) continue;
                
                set[index] = true;
                dependencies[count++] = GetDependencyInfo(Unsafe.As<TDependency>(members[index]));
            }


            ///////////////////////////////////////////////////////////////////
            // Validate and create pipeline
            if (0 == count) return downstream;
            if (dependencies.Length > count) Array.Resize(ref dependencies, count);

            return (ref PipelineContext context) =>
            {
                ResolverOverride? @override;
                PipelineContext local;
                ErrorInfo error;

                for (var index = 0; index < dependencies.Length; index++)
                {
                    ref var member = ref dependencies[index];

                    if (member.AllowDefault)
                    {
                        error = new ErrorInfo();
                        local = context.CreateContext(ref member.Contract, ref error);
                    }
                    else
                    {
                        local = context.CreateContext(ref member.Contract);
                    }

                    using var action = local.Start(member.Info);

                    var value = null != (@override = local.GetOverride(ref member))
                        ? local.GetValue(member.Info, @override.Value)
                        : local.Resolve(ref member);

                    if (!local.IsFaulted) SetValue(member.Info, context.Target!, value);
                }

                return downstream?.Invoke(ref context);
            };
        }


        #endregion

    }
}
