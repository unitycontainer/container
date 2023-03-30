using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        #region Selection

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

        public static ConstructorInfo? AlgorithmicSelector(ref TContext context, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors, SortPredicate);
            var container = context.Container;

            foreach (var info in constructors)
            {
                var parameters = info.GetParameters();
                if (parameters.All(p => p.HasDefaultValue || CanResolve(container, p)))
                {
                    return info;
                }
            }

            return null;
        }

        #endregion


        #region Implementation

        private static bool CanResolve(UnityContainer container, ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute<DependencyResolutionAttribute>();
            return attribute is null
                ? container.CanResolve(info.ParameterType, null)
                : container.CanResolve(attribute.ContractType ?? info.ParameterType,
                                       attribute.ContractName);
        }

        // Sort Predicate
        private static int SortPredicate(ConstructorInfo x, ConstructorInfo y)
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

                if (parameter.ParameterType.IsArray ||
                    parameter.ParameterType.IsGenericType)
                    sum += 1;

                if (parameter.ParameterType.IsByRef)
                    sum -= 1000;
            }

            return sum;
        }

        #endregion
    }
}
