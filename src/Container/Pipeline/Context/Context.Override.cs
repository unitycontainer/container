using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct BuilderContext
    {

        public ResolverOverride? GetOverride<TDescriptor, TMemberInfo>(ref TDescriptor descriptor)
            where TDescriptor : IImportDescriptor<TMemberInfo>
        {
            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var @override = Overrides[index];

                // Match member first
                if (@override is IMatch<TMemberInfo, MatchRank> candidate)
                {
                    rank = candidate.Match(descriptor.MemberInfo);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }

                    continue;
                }

                if (@override is IMatchImport dependency)
                {
                    rank = dependency.MatchImport(ref descriptor);

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
