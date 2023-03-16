using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    public partial class MethodStrategy
    {
        protected override int SelectMember(InjectionMember<MethodInfo, object[]> method, MethodInfo[] members, ref Span<int> indexes)
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
    }
}
