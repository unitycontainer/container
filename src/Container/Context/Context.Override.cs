using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct BuilderContext
    {
        public ResolverOverride? GetOverride<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IImportDescriptor<TMemberInfo>
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var @override = Overrides[index];

                // Match member first
                if (@override is IMatch<TMemberInfo> candidate)
                {
                    rank = candidate.RankMatch(descriptor.MemberInfo);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }

                    continue;
                }

                if (@override is IMatchImport<TMemberInfo> secondary)
                {
                    rank = secondary.Matches(descriptor.MemberInfo, 
                                             descriptor.ContractType, 
                                             descriptor.ContractName);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
                return candidateOverride;

            return null;
        }
    }
}
