using System;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ResolverOverride? GetOverride<TContext>(ref TContext context, in ImportInfo<TDependency> import)
            where TContext : IBuilderContext
        {
            var length = context.Overrides.Length;
            if (0 == length--) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;

            for (var index = length; index >= 0; --index)
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
                    rank = dependency.MatchImport(in import);

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

        protected bool GetOverride<TContext>(ref TContext context, in ImportInfo<TDependency> import, out object? value)
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

                    if (MatchRank.ExactMatch == rank)
                    { 
                        value = @override.Value;
                        return true;
                    }

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }

                    continue;
                }

                if (@override is IMatchImport dependency)
                {
                    rank = dependency.MatchImport(in import);

                    if (MatchRank.ExactMatch == rank)
                    {
                        value = @override.Value;
                        return true;
                    }

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
            { 
                value = candidateOverride.Value;
                return true;
            }

            value = UnityContainer.NoValue;
            return false;
        }

    }
}
