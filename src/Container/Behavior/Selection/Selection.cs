using System;
using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Selection
    {
        #region Fields

        private static MatchDelegate<object, ParameterInfo, MatchRank> MatchTo 
            = Unity.Resolution.Matching.MatchTo;

        #endregion


        #region Setup

        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Default Constructor selection algorithm. This method determines which
            // Constructor is selected when there are multiple choices and noting is annotated.
            policies.Set<ConstructorSelector>(SelectConstructor);

            
            // Matches MemberInfo and injected data
            policies.Set<MemberSelector<FieldInfo, object>>(SelectInjectedField);
            policies.Set<MemberSelector<MethodInfo, object[]>>(SelectInjectedMethod);
            policies.Set<MemberSelector<PropertyInfo, object>>(SelectInjectedProperty);
            policies.Set<MemberSelector<ConstructorInfo, object[]>>(SelectInjectedConstructor);


            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            policies.Set<DeclaredMembers<ConstructorInfo>>(GetConstructors);
            policies.Set<DeclaredMembers<PropertyInfo>>(GetProperties);
            policies.Set<DeclaredMembers<MethodInfo>>(GetMethods);
            policies.Set<DeclaredMembers<FieldInfo>>(GetFields);


            // Subscribe to change notifications
            MatchTo = policies.Get<MatchDelegate<object, ParameterInfo, MatchRank>>(OnMatchToChanged)!;
        }

        #endregion


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
                var compatibility = (int)MatchTo(data![i], parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }


        private static void OnMatchToChanged(System.Type? target, System.Type type, object? policy)
            => MatchTo = (MatchDelegate<object, ParameterInfo, MatchRank>)(policy
                ?? throw new ArgumentNullException(nameof(policy)));
        
        #endregion
    }
}
