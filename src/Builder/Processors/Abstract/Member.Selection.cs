using Unity.Dependency;
using Unity.Injection;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData> 
    {
        protected virtual Enumerator<TMemberInfo> SelectMembers(ref TContext context, TMemberInfo[] members)
        {
            var injections = GetInjectedMembers(context.Registration);

            if (injections is null || 0 == injections.Length)
                return new Enumerator<TMemberInfo>(GetMemberType, ProvideInjectionInfo, members);

            int index, current = 0;
            var set = new InjectionMember<TMemberInfo, TData>[members.Length];

            // Match injections with members
            foreach (var member in injections)
            {
                current += 1;

                if (-1 == (index = MatchMember(member, members)))
                {
                    context.Error($"{member} doesn't match any members on type {context.Type}");
                    return default;
                }

                ref var position = ref set[index];

                if (position is not null) continue;

                position = member;
            }

            return new Enumerator<TMemberInfo>(GetMemberType, ProvideInjectionInfo, members, set);
        }

        protected virtual int MemberMatch(InjectionMember<TMemberInfo, TData> member, TMemberInfo[] members)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < members.Length; index++)
            {
                var field = members[index];
                var match = member.RankMatch(field);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (member.Data is IMatchInfo<TMemberInfo> iMatch)
                    match = iMatch.RankMatch(field);

                if (match > bestSoFar)
                {
                    position = index;
                    bestSoFar = match;
                }
            }

            return position;
        }
    }
}
