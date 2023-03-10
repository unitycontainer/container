using System;
using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Matching
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Match types
            policies.Set<MatchDelegate<Type, Type, MatchRank>>(MatchTo);

            // Match Array to Type
            policies.Set<MatchDelegate<Array, Type, MatchRank>>(MatchTo);

            // Match injected data to array of MethodBase members
            policies.Set<MatchDelegate<object[], MethodBase, int>>(MatchTo);

            // Match object to Type
            policies.Set<MatchDelegate<object, Type, MatchRank>>(MatchTo);

            // Match object to ParameterInfo
            policies.Set<MatchDelegate<object, ParameterInfo, MatchRank>>(MatchTo);
        }
    }
}
