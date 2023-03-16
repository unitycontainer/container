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

/* Unmerged change from project 'Unity.Container (net7.0)'
Before:
            policies.Set<MatchDelegate<Type, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<Type, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/

/* Unmerged change from project 'Unity.Container (net6.0)'
Before:
            policies.Set<MatchDelegate<Type, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<Type, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/
            policies.Set<MatchDelegate<Type, Type, MatchRank>>(Resolution.Matching.MatchTo);

            // Match Array to Type

/* Unmerged change from project 'Unity.Container (net7.0)'
Before:
            policies.Set<MatchDelegate<Array, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<Array, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/

/* Unmerged change from project 'Unity.Container (net6.0)'
Before:
            policies.Set<MatchDelegate<Array, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<Array, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/
            policies.Set<MatchDelegate<Array, Type, MatchRank>>(Resolution.Matching.MatchTo);

            // Match injected data to array of MethodBase members
            policies.Set<MatchDelegate<object[], MethodBase, int>>(Resolution.Matching.MatchTo);

            // Match object to Type

/* Unmerged change from project 'Unity.Container (net7.0)'
Before:
            policies.Set<MatchDelegate<object, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<object, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/

/* Unmerged change from project 'Unity.Container (net6.0)'
Before:
            policies.Set<MatchDelegate<object, Type, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<object, Type, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/
            policies.Set<MatchDelegate<object, Type, MatchRank>>(Resolution.Matching.MatchTo);

            // Match object to ParameterInfo

/* Unmerged change from project 'Unity.Container (net7.0)'
Before:
            policies.Set<MatchDelegate<object, ParameterInfo, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<object, ParameterInfo, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/

/* Unmerged change from project 'Unity.Container (net6.0)'
Before:
            policies.Set<MatchDelegate<object, ParameterInfo, Resolution.MatchRank>>(Resolution.Matching.MatchTo);
After:
            policies.Set<MatchDelegate<object, ParameterInfo, Resolution.Algorithm.MatchRank>>(Resolution.Matching.MatchTo);
*/
            policies.Set<MatchDelegate<object, ParameterInfo, MatchRank>>(Resolution.Matching.MatchTo);
        }
    }
}
