using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext>
    {
        protected override int MemberMatch(InjectionMember<MethodInfo, object[]> method, MethodInfo[] members)
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
                var compatibility = MatchTo(method.Data, member);
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
