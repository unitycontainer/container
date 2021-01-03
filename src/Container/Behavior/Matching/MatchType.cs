using System;

namespace Unity.Container
{
    internal static partial class Matching
    {
        public static MatchRank MatchTo(this Type type, Type match)
        {
            if (typeof(Type).Equals(match))
                return MatchRank.ExactMatch;

            if (type == match || Nullable.GetUnderlyingType(type) == match)
                return MatchRank.HigherProspect;

            if (match.IsAssignableFrom(type))
                return MatchRank.Compatible;

            if (typeof(Array) == type && match.IsArray)
                return MatchRank.HigherProspect;

            if (type.IsGenericType && type.IsGenericTypeDefinition && match.IsGenericType &&
                type.GetGenericTypeDefinition() == match.GetGenericTypeDefinition())
                return MatchRank.ExactMatch;

            return MatchRank.NoMatch;
        }
    }
}
