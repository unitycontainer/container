using System;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        #region Fields

        private static MatchDelegate<object, ParameterInfo, MatchRank> MatchTo 
            = Matching.MatchTo;

        #endregion


        #region Setup

        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Default Constructor selection algorithm. This method determines which
            // Constructor is selected when there are multiple choices and noting is annotated.
            policies.Set<ConstructorInfo, SelectorDelegate<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(SelectConstructor);

            
            // Matches MemberInfo and injected data
            policies.Set<FieldInfo, SelectorDelegate<InjectionMember<FieldInfo, object>, FieldInfo[], int>>(SelectInjectedField);
            policies.Set<MethodInfo, SelectorDelegate<InjectionMember<MethodInfo, object[]>, MethodInfo[], int>>(SelectInjectedMethod);
            policies.Set<PropertyInfo, SelectorDelegate<InjectionMember<PropertyInfo, object>, PropertyInfo[], int>>(SelectInjectedProperty);
            policies.Set<ConstructorInfo, SelectorDelegate<InjectionMember<ConstructorInfo, object[]>, ConstructorInfo[], int>>(SelectInjectedConstructor);


            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            // These selectors are used by Build strategies to get declared members
            policies.Set<ConstructorInfo, DeclaredMembers<ConstructorInfo>>(GetConstructors);
            policies.Set<PropertyInfo, DeclaredMembers<PropertyInfo>>(GetProperties);
            policies.Set<MethodInfo, DeclaredMembers<MethodInfo>>(GetMethods);
            policies.Set<FieldInfo, DeclaredMembers<FieldInfo>>(GetFields);


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
