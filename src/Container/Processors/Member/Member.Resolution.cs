using System;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override ResolveDelegate<PipelineContext>? Build(ref Pipeline_Builder<ResolveDelegate<PipelineContext>?> builder)
        {
            Type type = builder.Context.Type;
            var members = GetSupportedMembers(type);
            var downstream = builder.Build();

            // Check if any methods are available
            if (0 == members.Length) return downstream;

            int count = 0;
            //ReflectionInfo<TMemberInfo>[]? imports = null;
            var injected = (builder.Context.Registration as ISequenceSegment<InjectionMemberInfo<TMemberInfo>>)?.Next;
            var injections = injected;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];

                // Injection first
                //while (null != injected)
                //{
                //    if (MatchRank.ExactMatch == injected.Match(member))
                //    {
                //        (imports ??= new ReflectionInfo<TMemberInfo>[members.Length - index])[count++] = injected.FillReflectionInfo(member);

                //        goto InitializeNext;
                //    }

                //    injected = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injected.Next);
                //}

                // Annotation second
                //var attribute = GetImportAttribute(Unsafe.As<TDependency>(member));
                //if (attribute is null) continue;

                //(imports ??= new ReflectionInfo<TMemberInfo>[members.Length - index])[count++] 
                //    = new ReflectionInfo<TMemberInfo>(member, attribute.ContractType ?? MemberType(Unsafe.As<TDependency>(member)),
                //                                             attribute.ContractName,
                //                                             attribute.AllowDefault);
                // Rewind for the next member
                InitializeNext: injected = injections;
            }

            // Validate and trim dependency array
            //if (imports is null || 0 == count) return downstream;
            //if (imports.Length > count) Array.Resize(ref imports, count);

            // Create pipeline
            return (ref PipelineContext context) =>
            {
                //Debug.Assert(null != context.Target, "Should never be null");
                //ResolverOverride? @override;

                //for (var index = 0; index < imports.Length; index++)
                //{
                //    ref var info = ref imports[index];

                //    // Check for override
                //    if (null != (@override = context.GetOverride(in info.Import)))
                //        Build(ref context, in info.Import, AsImportData(Unsafe.As<TDependency>(info.Import.Member), @override.Value));
                //    else
                //        Build(ref context, in info.Import, in info.Data);
                //}

                return downstream?.Invoke(ref context);
            };
        }
    }
}
