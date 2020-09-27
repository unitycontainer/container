using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        private bool TryGetOverride(ref PipelineContext context, ResolverOverride[] overrides)
        {
            ResolverOverride? candidate = null;
            TDependency info = (TDependency)context.Action!;
            MatchRank matchRank = MatchRank.NoMatch;

            using var action = context.Start(overrides);

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                // Check if this parameter is overridden
                if (((IEquatable<TDependency>)@override).Equals(info))
                {
                    var selected = context.Start(@override);

                    return selected.Success(GetOverrideValue(ref context, info, @override.Value));
                }

                // Check if dependency override for the contract is provided
                var match = @override.MatchTo(in context.Contract);
                if (MatchRank.NoMatch == match) continue;

                if (match > matchRank)
                {
                    matchRank = match;
                    candidate = @override;
                }
            }

            // No exact matches but this one is compatible
            if ((null != candidate) &&
                ((candidate.RequireExactMatch && (MatchRank.ExactMatch == matchRank)) ||
                (!candidate.RequireExactMatch && (MatchRank.NoMatch != matchRank))))
            {
                var matched = context.Start(candidate);
                
                return matched.Success(GetOverrideValue(ref context, info, candidate.Value));
            }

            return false;
        }

        private object? GetOverrideValue(ref PipelineContext context, TDependency info, object? value)
        {
            return value switch
            {
                IResolve iResolve                         => GetOverrideValue(ref context, info, iResolve.Resolve(ref context)),

                ResolveDelegate<PipelineContext> resolver => GetOverrideValue(ref context, info, resolver(ref context)),

                IResolverFactory<TDependency> infoFactory => GetOverrideValue(ref context, info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                                            .Invoke(ref context)),
                IResolverFactory<Type> typeFactory        => GetOverrideValue(ref context, info, typeFactory.GetResolver<PipelineContext>(context.Type)
                                                                                                            .Invoke(ref context)),
                _ => value,
            };
        }

    }
}
