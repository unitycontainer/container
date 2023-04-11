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

                case IResolve iResolve:
                    return new ValueData(iResolve.Resolve(ref this), DataType.Value);

                case ResolverPipeline resolver:
                    return new ValueData(resolver(ref this), DataType.Value);

                case IResolverFactory<TMemberInfo> factory:
                    return new ValueData(factory.GetResolver<BuilderContext>(member)(ref this), DataType.Value);

                case IResolverFactory<Type> factory:
                    return new ValueData(factory.GetResolver<BuilderContext>(contract.Type)(ref this), DataType.Value);

                case ResolverFactory<BuilderContext> factory:
                    return new ValueData(factory(contract.Type)(ref this), DataType.Value);

                //case Type target when typeof(Type) != info.MemberType:
                //    info.ContractType = target;
                //    info.ContractName = null;
                //    info.AllowDefault = false;
                //    info.DefaultValue = default;
                //    data = default;
                //    return;

                case UnityContainer.InvalidValue _:
                    return default;

                case null when DataType.None == data.Type:
                case Array when DataType.Array == data.Type:
                    return default;

                default:
                    data.Type = DataType.Value;
                    return data;
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
