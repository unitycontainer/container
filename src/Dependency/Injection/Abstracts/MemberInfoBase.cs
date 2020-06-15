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



        #region Implementation

        public override bool Match(TMemberInfo? other)
        {
            if (null == other) return false;

            if (null != Info)
            {
                if (Info.Equals(other)) return true;

                return false;
            }

            if (Name != other.Name) return false;
            
            return true;
        }

        #endregion


        #region Debug

        protected override string ToString(bool debug = false) => null == Selection
            ? $"{GetType().Name}: {Name}"
            : $"{GetType().Name}: {Selection.DeclaringType}.{Name}";

        #endregion
    }
}
