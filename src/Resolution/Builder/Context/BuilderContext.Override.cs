using Unity.Dependency;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        public ValueData GetOverride<TMemberInfo>(TMemberInfo info, ref Contract contract)
        {
            var @override = GetResolverOverride(info, ref contract);
            if (@override is null) return default;

            var value = @override.Resolve(ref this);

            return default;
        }

        public ResolverOverride? GetResolverOverride<TMemberInfo>(TMemberInfo info, ref Contract contract)
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidate = null;
            MatchRank rank, bestSoFar = MatchRank.NoMatch;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var @override = Overrides[index];

                rank = @override switch
                {
                    // Field, Property or Parameter overrides
                    IMatchInfo<TMemberInfo> member => member.RankMatch(info),

                    // Contract override
                    IMatchContract<TMemberInfo> depend => depend.RankMatch(info, contract.Type,
                                                                                 contract.Name),
                    // Something else
                    _ => MatchRank.NoMatch,
                };

                if (MatchRank.ExactMatch == rank) return @override;
                if (rank <= bestSoFar) continue;

                bestSoFar = rank;
                candidate = @override;
            }

            if (candidate is not null && candidate.Equals(bestSoFar))
                return candidate;

            return null;
        }
    }
}
