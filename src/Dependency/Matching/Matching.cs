using System;
using System.Reflection;

namespace Unity.Injection
{
    public static class Matching
    {
        public static MatchRank MatchTo(this object value, ParameterInfo parameter)
        {
            switch (value)
            {
                case null:
                    return !parameter.ParameterType.IsValueType || (null != Nullable.GetUnderlyingType(parameter.ParameterType))
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return MatchTo(array, parameter);

                case IMatch<ParameterInfo> iMatchParam:
                    return iMatchParam.Match(parameter);

                case Type type:
                    return MatchTo(type, parameter.ParameterType);
            }

            var objectType = value.GetType();

            if (objectType == parameter.ParameterType)
                return MatchRank.ExactMatch;

            return parameter.ParameterType.IsAssignableFrom(objectType) 
                ? MatchRank.Compatible : MatchRank.NoMatch;
        }

        public static MatchRank MatchTo(this Array array, ParameterInfo parameter)
        {
            var type = array.GetType();

            if (parameter.ParameterType == type)          return MatchRank.ExactMatch;
            if (parameter.ParameterType == typeof(Array)) return MatchRank.HigherProspect;

            return parameter.ParameterType
                            .IsAssignableFrom(type)
                ? MatchRank.Compatible
                : MatchRank.NoMatch;
        }

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
