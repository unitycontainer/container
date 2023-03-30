using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
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

        // Sort Predicate
        private static int DefaultSortPredicate(ConstructorInfo x, ConstructorInfo y)
        {
            int match;

            var xParams = x.GetParameters();
            var yParams = y.GetParameters();

            if (0 != (match = yParams.Length - xParams.Length)) return match;
            if (0 != (match = RankByComplexity(yParams) - RankByComplexity(xParams))) return match;

            return 0;
        }

        private static int RankByComplexity(ParameterInfo[] parameters)
        {
            var sum = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.HasDefaultValue)
                    sum += 1;

                if (parameter.ParameterType.IsArray || parameter.ParameterType.IsGenericType)
                    sum += 1;

                if (parameter.ParameterType.IsByRef)
                    sum -= 1000;
            }

            return sum;
        }
    }
}
