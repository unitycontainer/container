using System;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
using Unity.Resolution;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        private static class Fields
        {
            #region Injection

            public static InjectionMember GetInjectionDefault()
                => new InjectionField(FieldName);

            public static InjectionMember GetInjectionValue(object argument)
                => new InjectionField(FieldName, argument);

            public static InjectionMember GetInjectionArgs(params object[] arguments)
                => throw new NotSupportedException();

            public static InjectionMember GetInjectionContract(Type type, string name)
#if UNITY_V5
                => new InjectionField(FieldName, new ResolvedParameter(type, name));
#else
                => new InjectionField(FieldName, type, name);
#endif

            public static InjectionMember GetInjectionDefaultOptional()
#if UNITY_V5
                => new InjectionField(FieldName, ResolutionOption.Optional);
#else
                => new OptionalField(FieldName);
#endif

            public static InjectionMember GetInjectionValueOptional(object argument)
#if UNITY_V5
                => new InjectionField(FieldName, argument);
#else
                => new OptionalField(FieldName, argument);
#endif

            public static InjectionMember GetInjectionContractOptional(Type type, string name)
#if UNITY_V5
                => new InjectionField(FieldName, new OptionalParameter(type, name));
#else
                => new OptionalField(FieldName, type, name);
#endif

            #endregion


            #region Override

            public static ResolverOverride GetMemberOverride(object value)
                => new FieldOverride(FieldName, value);

            public static ResolverOverride GetMemberOverrideByName(string name, object value)
                => new FieldOverride(name, value);

            public static ResolverOverride GetMemberOverrideByType(Type type, object value)
                => throw new NotSupportedException();

            public static ResolverOverride GetMemberOverrideWithContract(Type _, object value)
                => throw new NotSupportedException();

            #endregion
        }
    }
}
