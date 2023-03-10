using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Matching
    {
        public static int MatchTo(object[]? data, MethodBase? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)data![i].MatchTo(parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }


        public static MatchRank MatchTo(this Array array, Type target)
        {
            var type = array.GetType();

            if (target == type) return MatchRank.ExactMatch;
            if (target == typeof(Array)) return MatchRank.HigherProspect;

            return target.IsAssignableFrom(type)
                ? MatchRank.Compatible
                : MatchRank.NoMatch;
        }
    }
}
