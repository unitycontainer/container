using System;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        /// <summary>
        /// Returns override matching this dependency 
        /// </summary>
        /// or <see cref="PropertyInfo"/></typeparam>
        /// <returns><see cref="ResolverOverride"/> object or null</returns>
        public ResolverOverride? GetOverride<T>(ref DependencyInfo<T> dependency)
        {
            if (0 == Overrides.Length) return null;

            ResolverOverride? candidateOverride = null;
            MatchRank candidateRank = MatchRank.NoMatch;
            var overrides = Overrides;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                var rank = Unsafe.As<IMatchContract<T>>(@override)
                                 .Match(dependency.Info, in dependency.Contract);

                if (MatchRank.ExactMatch == rank) return @override;

                if (rank > candidateRank)
                {
                    candidateRank = rank;
                    candidateOverride = @override;
                }
            }

            if (null != candidateOverride && candidateRank >= candidateOverride.RequireRank)
                return candidateOverride;

            return null;
        }

        public object? GetValue<TDependency>(TDependency info, object? value)
        {
            return value switch
            {
                ResolveDelegate<PipelineContext> resolver => GetValue(info, resolver(ref this)),

                IResolve iResolve                         => GetValue(info, iResolve.Resolve(ref this)),

                IResolverFactory<TDependency> infoFactory => GetValue(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                       .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => GetValue(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                       .Invoke(ref this)),
                _ => value,
            };
        }
    }
}
