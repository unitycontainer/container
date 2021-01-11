using System.Diagnostics;
using System.Runtime.CompilerServices;
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

            var sequence = context.Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>;
            var injection = sequence?.Next;

            if (0 == members.Length) return;

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                var import = new ImportDescriptor<TMemberInfo>(members[i]);

                // Load attributes
                DescribeMember(ref import);

                // Injection, if exists
                while (null != injection)
                {
                    var match = injection.Match(Unsafe.As<TMemberInfo>(import.MemberInfo));

                    if (MatchRank.NoMatch != match)
                    {
                        if (MatchRank.ExactMatch != match)
                        {
                            if (injection.Data is IMatch<TMemberInfo, MatchRank> iMatch &&
                                iMatch.Match(import.MemberInfo) is MatchRank.NoMatch)
                                continue;
                        }

                        injection.DescribeImport(ref import);

                        goto activate;
                    }

                    injection = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injection.Next);
                }

                // Attribute
                if (!import.IsImport) goto next;

                activate: Build(ref context, ref import);

                // Rewind for the next member
                next: injection = sequence?.Next;
            }
        }
    }
}
