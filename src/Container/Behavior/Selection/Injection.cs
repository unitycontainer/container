using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        public static int SelectInjectedConstructor(InjectionMember<ConstructorInfo, object[]> member, ConstructorInfo[] members, ref Span<int> indexes)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var compatibility = CompareTo(member.Data, members[index]);

                if (0 == compatibility) return index;

                if (bestSoFar < compatibility)
                {
                    position = index;
                    bestSoFar = compatibility;
                }
            }

            return position;
        }


        public static int SelectInjectedMethod(InjectionMember<MethodInfo, object[]> method, MethodInfo[] members, ref Span<int> indexes)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                if (method.Name != member.Name) continue;

                if (-1 == bestSoFar && method.Data is null)
                {   // If no data, match by name
                    bestSoFar = 0;
                    position = index;
                }

                // Calculate compatibility
                var compatibility = CompareTo(method.Data, member);
                if (0 == compatibility) return index;

                if (bestSoFar < compatibility)
                {
                    position = index;
                    bestSoFar = compatibility;
                }
            }

            return position;
        }


        private static int SelectInjectedField(InjectionMember<FieldInfo, object> injection, FieldInfo[] fields, ref Span<int> indexes)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var match = injection.Match(field);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (injection.Data is IMatch<FieldInfo, MatchRank> iMatch)
                    match = iMatch.Match(field);

                if (match > bestSoFar)
                {
                    position = index;
                    bestSoFar = match;
                }
            }

            return position;
        }


        public static int SelectInjectedProperty(InjectionMember<PropertyInfo, object> injection, PropertyInfo[] properties, ref Span<int> indexes)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                var match = injection.Match(property);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (injection.Data is IMatch<PropertyInfo, MatchRank> iMatch)
                    match = iMatch.Match(property);

                if (match > bestSoFar)
                {
                    position = index;
                    bestSoFar = match;
                }
            }

            return position;
        }
    }
}
