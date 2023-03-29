using Unity.Dependency;
using Unity.Injection;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData>
    {
        protected virtual int SelectMember(InjectionMember<TMemberInfo, TData> injection, TMemberInfo[] fields, ref Span<int> indexes)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var match = injection.RankMatch(field);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (injection.Data is IMatchInfo<TMemberInfo> iMatch)
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
