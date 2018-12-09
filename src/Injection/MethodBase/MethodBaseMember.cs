using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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

        //public abstract TMemberInfo GetInfo(Type type);

        public virtual object[] GetParameters() => null;
    }
}
