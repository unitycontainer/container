using System;
using System.Reflection;
using Unity.Dependency;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {

        protected static int MatchTo(object[]? data, MethodBase? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)MatchTo(data![i], parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }

        protected static MatchRank MatchTo(object value, ParameterInfo parameter)
        {
            switch (value)
            {
                case null:
                    return !parameter.ParameterType.IsValueType || null != Nullable.GetUnderlyingType(parameter.ParameterType)
                         ? MatchRank.ExactMatch : MatchRank.NoMatch;

                case Array array:
                    return array.MatchTo(parameter.ParameterType);

                case IMatchInfo<ParameterInfo> iMatchParam:
                    return iMatchParam.RankMatch(parameter);

                case Type type:
                    return type.MatchTo(parameter.ParameterType);

                case IResolve:
                case IResolverFactory:
                    return MatchRank.HigherProspect;
            }

            var objectType = value.GetType();

            if (objectType == parameter.ParameterType)
                return MatchRank.ExactMatch;

            return parameter.ParameterType.IsAssignableFrom(objectType)
                ? MatchRank.Compatible : MatchRank.NoMatch;
        }
    }
}
