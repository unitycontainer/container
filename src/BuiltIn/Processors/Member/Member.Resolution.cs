using System;
using System.Diagnostics;
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
            InjectionInfo<TMemberInfo>[]? imports = null;
            var injected = GetInjected<InjectionMemberInfo<TMemberInfo>>(builder.Context.Registration);
            var injections = injected;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];

                // Injection first
                while (null != injected)
                {
                    if (MatchRank.ExactMatch == injected.Match(member))
                    {
                        (imports ??= new InjectionInfo<TMemberInfo>[members.Length - index])[count++] = injected.GetInfo(member);

                        goto InitializeNext;
                    }

                    injected = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injected.Next);
                }

                // Annotation second
                var attribute = GetImportAttribute(Unsafe.As<TDependency>(member));
                if (null == attribute) continue;

                (imports ??= new InjectionInfo<TMemberInfo>[members.Length - index])[count++] 
                    = new InjectionInfo<TMemberInfo>(member, attribute.ContractType ?? MemberType(Unsafe.As<TDependency>(member)),
                                                             attribute.ContractName,
                                                             attribute.AllowDefault);
                // Rewind for the next member
                InitializeNext: injected = injections;
            }

            // Validate and trim dependency array
            if (null == imports || 0 == count) return downstream;
            if (imports.Length > count) Array.Resize(ref imports, count);

            // Create pipeline
            return (ref PipelineContext context) =>
            {
                Debug.Assert(null != context.Target, "Should never be null");
                ResolverOverride? @override;

                for (var index = 0; index < imports.Length; index++)
                {
                    ref var info = ref imports[index];

                    // Check for override
                    if (null != (@override = context.GetOverride(in info.Import)))
                        Build(ref context, in info.Import, info.Import.Info.AsImportData(@override.Value));
                    else
                        Build(ref context, in info.Import, in info.Data);
                }

                return downstream?.Invoke(ref context);
            };
        }
    }
}
