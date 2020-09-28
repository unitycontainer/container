using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext 
    {
        public ResolverOverride? GetOverride<TDependency>(TDependency info, in Contract contract)
        {
            var overrides = Overrides;

            if (null == overrides) return null;

            ResolverOverride? candidate = null;
            MatchRank matchRank = MatchRank.NoMatch;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                // Check if exact match
                if (((IEquatable<TDependency>)@override).Equals(info))
                { 
                    return @override;
                }

                // Check if close enough
                var match = @override.MatchTo(in contract);
                if (MatchRank.NoMatch == match) continue;

                if (match > matchRank)
                {
                    matchRank = match;
                    candidate = @override;
                }
            }

            if ((null != candidate) && // Not an exact matches but this one is compatible
                ((candidate.RequireExactMatch && (MatchRank.ExactMatch == matchRank)) ||
                (!candidate.RequireExactMatch && (MatchRank.NoMatch    != matchRank))))
            { 
                return candidate;
            }

            return null;
        }
    }
}
