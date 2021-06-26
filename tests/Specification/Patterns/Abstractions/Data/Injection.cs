using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Resolution;
using Unity.Injection;
using Unity;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        #region Fields

        protected static Func<object, InjectionMember>       InjectionMember_Value;
        protected static Func<object[], InjectionMember>     InjectionMember_Args;
        protected static Func<InjectionMember>               InjectionMember_Default;
        protected static Func<Type, string, InjectionMember> InjectionMember_Contract;

        protected static Func<object, ResolverOverride>         MemberOverride;
        protected static Func<string, object, ResolverOverride> MemberOverride_ByName;
        protected static Func<Type, object, ResolverOverride>   MemberOverride_ByType;
        protected static Func<Type, object, ResolverOverride>   MemberOverride_ByContract;

        #endregion

        protected static void LoadInjectionProxies()
        {
            Type support = Type.GetType($"{typeof(PatternBase).FullName}+{Member}");
            
            if (support is null) return;


            InjectionMember_Value = (Func<object, InjectionMember>)support
                .GetMethod("GetInjectionValue").CreateDelegate(typeof(Func<object, InjectionMember>));

            InjectionMember_Args = (Func<object[], InjectionMember>)support
                .GetMethod("GetInjectionArgs").CreateDelegate(typeof(Func<object[], InjectionMember>));

            InjectionMember_Default = (Func<InjectionMember>)support
                .GetMethod("GetInjectionDefault").CreateDelegate(typeof(Func<InjectionMember>));

            InjectionMember_Contract = (Func<Type, string, InjectionMember>)support
                .GetMethod("GetInjectionContract").CreateDelegate(typeof(Func<Type, string, InjectionMember>));

            MemberOverride = (Func<object, ResolverOverride>)support
                .GetMethod("GetMemberOverride").CreateDelegate(typeof(Func<object, ResolverOverride>));

            MemberOverride_ByName = (Func<string, object, ResolverOverride>)support
                .GetMethod("GetMemberOverrideByName").CreateDelegate(typeof(Func<string, object, ResolverOverride>));

            MemberOverride_ByType = (Func<Type, object, ResolverOverride>)support
                .GetMethod("GetMemberOverrideByType").CreateDelegate(typeof(Func<Type, object, ResolverOverride>));

            MemberOverride_ByContract = (Func<Type, object, ResolverOverride>)support
                .GetMethod("GetMemberOverrideWithContract").CreateDelegate(typeof(Func<Type, object, ResolverOverride>));
        }
    }
}
