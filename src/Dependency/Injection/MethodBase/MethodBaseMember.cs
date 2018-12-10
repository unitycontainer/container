using System;
using Unity.Injection;

namespace Unity
{
    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember
    {
        #region Fields


        
        #endregion


        public TMemberInfo Info
        {
            get;
            protected set;
        }

        public abstract TMemberInfo GetInfo(Type type);

        public abstract object[] GetParameters();
    }
}
