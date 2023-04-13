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

            var data = new ValueData(@override.Resolve(ref this), DataType.Unknown);

            switch (data.Value)
            {
                case IInjectionInfoProvider<TMemberInfo> provider:
                    var memberInfo = new InjectionInfoStruct<TMemberInfo>(member, ref contract);
                    provider.ProvideInfo(ref memberInfo);
                    Resolve(ref memberInfo);
                    return memberInfo.InjectedValue;

                case IInjectionInfoProvider provider:
                    var info = new InjectionInfoStruct<TMemberInfo>(member, ref contract);
                    provider.ProvideInfo(ref info);
                    Resolve(ref info);
                    return info.InjectedValue;

                case IResolverFactory<TMemberInfo> factory:
                    return new ValueData(factory.GetResolver<BuilderContext>(member)(ref this), DataType.Value);

                case UnityContainer.InvalidValue _:
                    return default;

                case null when DataType.None == data.Type:
                case Array when DataType.Array == data.Type:
                    return default;

                default:
                    data.Type = DataType.Value;
                    break;
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
                if (MatchRank.ExactMatch != @override.RankMatch(Type))
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

        // TODO: Unused
        public object? GetValueRecursively<TInfo>(TInfo info, object? value)
        {
            return value switch
            {
                IResolve iResolve                   => GetValueRecursively(info, iResolve.Resolve(ref this)),

                ResolverPipeline resolver           => GetValueRecursively(info, resolver(ref this)),

                IResolverFactory<Type> typeFactory  => GetValueRecursively(info, typeFactory.GetResolver<BuilderContext>(Type)
                                                                                            .Invoke(ref this)),
                IResolverFactory<TInfo> infoFactory => GetValueRecursively(info, infoFactory.GetResolver<BuilderContext>(info)
                                                                                            .Invoke(ref this)),
                _ => value,
            };
        }

    }
}
