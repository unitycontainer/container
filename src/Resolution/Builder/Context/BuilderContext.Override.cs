using Unity.Dependency;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        public ValueData GetOverride<TMemberInfo>(TMemberInfo member, ref Contract contract)
        {
            var @override = GetResolverOverride(member, ref contract);
            if (@override is null) return default;

            var data = new ValueData(@override.Resolve(ref this), DataType.Value);

            if (data.Value is IInjectionInfoProvider provider)
            {
                var info = new InjectionInfoStruct<TMemberInfo>(member, ref contract);
                provider.ProvideInfo(ref info);
                if (DataType.Unknown == info.InjectedValue.Type)
                {
                    info.InjectedValue.Type = DataType.Value;
                    return info.InjectedValue;
                }

                Resolve(ref info);
                return info.InjectedValue;
            }

            return data;
        }

        public ResolverOverride? GetResolverOverride<TMemberInfo>(TMemberInfo info, ref Contract contract)
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidate = null;
            MatchRank rank, bestSoFar = MatchRank.NoMatch;

            for (var index = Overrides.Length - 1; index >= 0; --index)
            {
                var @override = Overrides[index];
                
                // Check if targeted type matches the current
                if (MatchRank.ExactMatch != @override.RankMatch(Parent.Type))
                    continue;

                rank = @override switch
                {
                    // Field, Property or Parameter overrides
                    IMatch<TMemberInfo> member => member.RankMatch(info),

                    // Contract override
                    IMatchContract depend => depend.RankMatch(ref contract),
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
