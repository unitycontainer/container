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
        private static class Properties
        {
            #region Injection

            public static InjectionMember GetInjectionDefault()
                => new InjectionProperty(PropertyName);

            public static InjectionMember GetInjectionValue(object argument)
                => new InjectionProperty(PropertyName, argument);

            public static InjectionMember GetInjectionArgs(params object[] arguments)
                => throw new NotSupportedException();

            public static InjectionMember GetInjectionContract(Type type, string name)
#if UNITY_V4
                => new InjectionProperty(PropertyName, new ResolvedParameter(type, name));
#elif UNITY_V5 || UNITY_V6
                => new InjectionProperty(PropertyName, new ResolvedParameter(type, name));
#else
                => new InjectionProperty(PropertyName, type, name);
#endif


            public static InjectionMember GetInjectionDefaultOptional()
#if UNITY_V4
                => throw new NotSupportedException();
#elif UNITY_V5 || UNITY_V6
                => new InjectionProperty(PropertyName, ResolutionOption.Optional);
#else
                => new OptionalProperty(PropertyName);
#endif

            public static InjectionMember GetInjectionValueOptional(object argument)
#if UNITY_V4
                => new InjectionProperty(PropertyName, argument);
#elif UNITY_V5 || UNITY_V6
                => new InjectionProperty(PropertyName, argument);
#else
                => new OptionalProperty(PropertyName, argument);
#endif

            public static InjectionMember GetInjectionContractOptional(Type type, string name)
#if UNITY_V4
                => new InjectionProperty(PropertyName, new OptionalParameter(type, name));
#elif UNITY_V5 || UNITY_V6
                => new InjectionProperty(PropertyName, new OptionalParameter(type, name));
#else
                => new OptionalProperty(PropertyName, type, name);
#endif

            #endregion


            #region Override

            public static ResolverOverride GetMemberOverride(object value)
                => new PropertyOverride(PropertyName, value);

            public static ResolverOverride GetMemberOverrideByName(string name, object value)
                => new PropertyOverride(name, value);

            public static ResolverOverride GetMemberOverrideByType(Type type, object value)
                => throw new NotSupportedException();

            public static ResolverOverride GetMemberOverrideWithContract(Type _, object value)
                => throw new NotSupportedException();

            #endregion
        }
    }
}
