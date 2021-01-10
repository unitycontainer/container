using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ResolverOverride? GetOverride<TContext>(ref TContext context, ref ImportDescriptor<TDependency> import)
            where TContext : IBuilderContext
        {
            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = context.Overrides.Length - 1; index >= 0; --index)
            {
                var @override = context.Overrides[index];

                // Match member first
                if (@override is IMatch<TDependency, MatchRank> candidate)
                {
                    rank = candidate.Match(import.MemberInfo);

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
                    rank = dependency.MatchImport(ref import);

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
