using Unity.Dependency;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        public ResolverOverride? GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<TMemberInfo>
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, bestSoFar = MatchRank.NoMatch;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var @override = Overrides[index];


                rank = @override switch
                {
                    // Check if any of Field, Property or Parameter overrides
                    IMatchInfo<TMemberInfo> member => member.RankMatch(descriptor.MemberInfo),

                    // Check if Dependency override
                    IMatchContract<TMemberInfo> depend => depend.RankMatch(descriptor.MemberInfo,
                                                                         descriptor.ContractType,
                                                                         descriptor.ContractName),
                    // Something unknown
                    _ => MatchRank.NoMatch,
                };

                if (MatchRank.ExactMatch == rank) return @override;
                if (rank <= bestSoFar) continue;

                bestSoFar = rank;
                candidateOverride = @override;
            }

            if (null != candidateOverride && candidateOverride.Equals(bestSoFar))
                return candidateOverride;

            return null;
        }
    }
}
