using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            ImportInfo<TDependency> import = default;
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
                DescribeImport(ref import);

                // Injection, if exists
                while (null != injection)
                {
                    var match = injection.Match(Unsafe.As<TMemberInfo>(import.MemberInfo));

                    if (MatchRank.NoMatch != match)
                    {
                        if (MatchRank.ExactMatch != match)
                        {
                            if (injection.Data is IMatch<TMemberInfo, MatchRank> iMatch &&
                                MatchRank.NoMatch == iMatch.Match(Unsafe.As<TMemberInfo>(import.MemberInfo)))
                            {
                                context.Error($"{injection.Data} is not compatible with {import.MemberInfo}");
                                return;
                            }
                        }

                        injection.DescribeImport(ref import);

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
                if (!import.IsImport) goto next;

                activate:

                var result = 0 < context.Overrides.Length && GetOverride(ref context, in import, out var @override)
                    ? FromUnknown(ref context, ref import, @override)
                    : import.ValueData.Type switch
                    {
                        ImportType.None     => FromContainer(ref context, ref import),
                        ImportType.Value    => import.ValueData,
                        ImportType.Unknown  => FromUnknown(ref context, ref import, import.ValueData.Value),
                        ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!),
                        _                   => default
                    }; ;

                if (result.IsValue)
                { 
                    try
                    {
                        SetValue(Unsafe.As<TDependency>(import.MemberInfo), context.Existing!, result.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        context.Error(ex.Message);
                    }
                    catch (Exception exception)
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
