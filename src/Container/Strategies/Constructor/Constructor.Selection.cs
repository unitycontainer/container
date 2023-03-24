using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    public partial class ConstructorStrategy
    {
        protected override int SelectMember(InjectionMember<ConstructorInfo, object[]> member, ConstructorInfo[] members, ref Span<int> indexes)
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
