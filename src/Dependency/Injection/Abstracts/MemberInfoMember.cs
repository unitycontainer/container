using System;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MemberInfoMember<TMemberInfo> : InjectionMember<TMemberInfo, object>
                                      where TMemberInfo : MemberInfo
    {
        #region Constructors

        protected MemberInfoMember(string name, object data) 
            : base(name, data)
        {
        }


        #endregion


        #region Overrides

        public override (TMemberInfo, object) FromType(Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var declaringType = MemberInfo.DeclaringType.GetTypeInfo();
            if (!declaringType.IsGenericType && !declaringType.ContainsGenericParameters)
                return ReferenceEquals(Data, ResolvedValue)
                    ? (MemberInfo, MemberInfo)
                    : (MemberInfo, Data);
#else
            if (MemberInfo.DeclaringType != null &&
               !MemberInfo.DeclaringType.IsGenericType &&
               !MemberInfo.DeclaringType.ContainsGenericParameters)
                return ReferenceEquals(Data, ResolvedValue)
                    ? (MemberInfo, MemberInfo)
                    : (MemberInfo, Data);
#endif
            var info = DeclaredMember(type, MemberInfo.Name);
            return ReferenceEquals(Data, ResolvedValue)
                ? (info, info)
                : (info, Data);
        }

        protected override void ValidateInjectionMember(Type type)
        {
            base.ValidateInjectionMember(type);

            if (null == Data || ReferenceEquals(Data, ResolvedValue)) return;

            if (!Matches(Data, MemberType))
            {
                throw new InvalidOperationException(
                    $"Type of injector {Name} does not match member type '{MemberType}'");
            }
        }

#if NETSTANDARD1_0
        public override bool Equals(TMemberInfo other)
        {
            return null != other && other.Name == Name;
        }
#endif
        #endregion


        #region Implementation

        protected abstract TMemberInfo DeclaredMember(Type type, string name);

        protected abstract Type MemberType { get; }

        #endregion
    }
}
