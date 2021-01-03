using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            #region Constructor Selection

            // Default Constructor selection algorithm. This method determines which
            // Constructor is selected when there are multiple choices and noting is annotated.
            
            policies.Set<ConstructorInfo, SelectorDelegate<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(
                ConstructorInfoFromType);
            
            // Matches ConstructorInfo and injected data
            
            policies.Set<ConstructorInfo, SelectorDelegate<InjectionMethodBase<ConstructorInfo>, ConstructorInfo[], int>>(
                ConstructorInfoFromInjected);

            // Matches MethodInfo and injected data
            
            policies.Set<MethodInfo, SelectorDelegate<InjectionMethodBase<MethodInfo>, MethodInfo[], int>>(
                MethodInfoFromInjected);

            #endregion


            #region Supported Members Selection

            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            // These selectors are used by Build strategies to get declared members

            policies.Set<ConstructorInfo, MembersDelegate<ConstructorInfo>>(GetConstructors);
            policies.Set<PropertyInfo, MembersDelegate<PropertyInfo>>(GetProperties);
            policies.Set<MethodInfo, MembersDelegate<MethodInfo>>(GetMethods);
            policies.Set<FieldInfo, MembersDelegate<FieldInfo>>(GetFields);

            #endregion
        }



        #region Implementation

        public static int CompareTo(object[]? data, MethodBase? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)data![i].MatchTo(parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }

        #endregion

    }
}
