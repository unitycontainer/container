using System;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MemberInfoBase<TMemberInfo> : InjectionMember<TMemberInfo, object>
                                    where TMemberInfo : MemberInfo
    {
        #region Constructors

        protected MemberInfoBase(string name, object data) 
            : base(name, data)
        {
        }


        protected MemberInfoBase(TMemberInfo info, object data)
            : base(info, data)
        {
        }

        #endregion


        #region Overrides

        public override TMemberInfo MemberInfo(Type type)
        {
            if (null == Selection) throw new InvalidOperationException($"{GetType().Name} is not initialized");

#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var declaringType = Selection.DeclaringType.GetTypeInfo();
            if (!declaringType.IsGenericType && !declaringType.ContainsGenericParameters)
                return Selection;
#else
            if (Selection.DeclaringType != null &&
               !Selection.DeclaringType.IsGenericType &&
               !Selection.DeclaringType.ContainsGenericParameters)
                return Selection;
#endif
            return DeclaredMember(type, Selection.Name) ?? 
                throw new InvalidOperationException($"Type {type} does not implement {Selection.Name}");
        }

        protected override TMemberInfo SelectMember(Type type, InjectionMember _)
        {
            foreach (var member in DeclaredMembers(type))
            {
                if (member.Name != Name) continue;

                return member;
            }

            throw new ArgumentException(NoMatchFound);
        }

        #endregion


        #region Implementation

        protected abstract TMemberInfo? DeclaredMember(Type type, string name);

        protected abstract Type? MemberType { get; }

        #endregion


        #region Debug

        protected override string ToString(bool debug = false) => null == Selection
            ? $"{GetType().Name}: {Name}"
            : $"{GetType().Name}: {Selection.DeclaringType}.{Name}";

        #endregion
    }
}
