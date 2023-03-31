using System.Reflection;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        protected virtual InjectionInfoStruct<ConstructorInfo> SelectConstructor(ref TContext context, ConstructorInfo[] members)
        {
            // Select injected or annotated constructor, if available
            var enumerator = SelectMembers(ref context, members);
            if (enumerator.MoveNext()) return enumerator.Current;

            // Only one constructor, nothing to select
            if (1 == members.Length)
            {
                var single = members[0];
                return new InjectionInfoStruct<ConstructorInfo>(single, single.DeclaringType!);
            }

            // Select using algorithm
            ConstructorInfo? selected = SelectAlgorithmically(ref context, members);
            if (null != selected)
            {
                return new InjectionInfoStruct<ConstructorInfo>(selected, selected.DeclaringType!);
            }

            context.Error($"No accessible constructors on type {context.Type}");
            return default;
        }


        #region Matching

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

        #endregion


        #region Implementation

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
