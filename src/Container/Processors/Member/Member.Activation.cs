using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");
            var members = GetMembers(context.Type);

            ResolverOverride? @override;
            ImportInfo import = default;
            var sequence = context.Registration as ISequenceSegment<InjectionMemberInfo<TMemberInfo>>;
            var injection = sequence?.Next;
            var remaining = sequence?.Length ?? 0;

            if (0 == members.Length)
            {
                if (injection is not null)
                    context.Error($"No accessible members on type {context.Type} matching {injection}");

                return;
            }

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                // Initialize member
                import.MemberInfo = Unsafe.As<TDependency>(members[i]);

                // Load attributes
                var attribute = LoadImportInfo(ref import);

                // Injection, if exists
                while (null != injection)
                {
                    var match = injection.Match(Unsafe.As<TMemberInfo>(import.MemberInfo));

                    if (MatchRank.NoMatch != match)
                    {
                        if (MatchRank.ExactMatch != match)
                        {
                            if (injection.Data is IMatch<TMemberInfo> iMatch &&
                                MatchRank.NoMatch == iMatch.Match(Unsafe.As<TMemberInfo>(import.MemberInfo)))
                            {
                                context.Error($"{injection.Data} is not compatible with {import.MemberInfo}");
                                return;
                            }
                        }

                        injection.GetImportInfo(ref import);
                        remaining -= 1;
                        goto activate;
                    }

                    injection = Unsafe.As<InjectionMemberInfo<TMemberInfo>>(injection.Next);
                }

                if (0 < remaining)
                {
                    context.Error($"Not all injection members were matched to {context.Type.Name} members");
                    return;
                }

                // Attribute
                if (ImportType.Attribute != attribute) goto next;

                activate:

                // Use override if provided
                if (null != (@override = GetOverride(in context, in import)))
                    ProcessImport(ref import, @override.Value);

                var result = import.Data.IsValue
                    ? import.Data
                    : Build(ref context, ref import);

                if (result.IsValue)
                {
                    try
                    {
                        SetValue(Unsafe.As<TDependency>(import.MemberInfo), context.Target!, result.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        context.Error(ex.Message);
                    }
                    catch(Exception exception)
                    {
                        context.Capture(exception);
                    }
                }

                // Rewind for the next member
                next: injection = sequence?.Next;
            }
        }
    }
}
