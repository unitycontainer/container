using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        protected override int MemberMatch(InjectionMember<ConstructorInfo, object[]> member, ConstructorInfo[] members)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var compatibility = MatchTo(member.Data, members[index]);

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
