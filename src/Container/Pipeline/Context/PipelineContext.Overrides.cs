using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        public ResolverOverride? GetOverride<T>(in ImportInfo<T> import)
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank rank, candidateRank = MatchRank.NoMatch;
            var overrides = Overrides;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                // Match member first
                if (@override is IMatch<T> candidate)
                {
                    rank = candidate.Match(import.Member);

                    if (MatchRank.ExactMatch == rank) return @override;

                    if (rank > candidateRank)
                    {
                        candidateRank = rank;
                        candidateOverride = @override;
                    }
                    
                    continue;
                }

                if (@override is IMatchImport<T> dependency)
                {
                    rank = dependency.Match(in import);

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

        public object? GetValueRecursively<TInfo>(TInfo info, object? value)
        {
            return value switch
            {
                ResolveDelegate<PipelineContext> resolver => GetValueRecursively(info, resolver(ref this)),

                IResolve iResolve                         => GetValueRecursively(info, iResolve.Resolve(ref this)),

                IResolverFactory<TInfo> infoFactory       => GetValueRecursively(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                       .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => GetValueRecursively(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                       .Invoke(ref this)),
                _ => value,
            };
        }
    }
}
