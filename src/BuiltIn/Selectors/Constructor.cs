using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace Unity.BuiltIn
{
    public static partial class Selectors
    {
        #region Selection

        private static ConstructorInfo? DefaultConstructorSelector(UnityContainer container, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors, SortPredicate);

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
            // TODO: Add support for ImportMany
            var attribute = info.GetCustomAttribute<ImportAttribute>();
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
