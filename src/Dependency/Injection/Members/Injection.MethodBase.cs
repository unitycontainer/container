using System.Reflection;

namespace Unity.Injection
{
    public abstract class InjectionMethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]> 
                                         where TMemberInfo : MethodBase
    {
        #region Constructors

        protected InjectionMethodBase(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        #endregion
    }
}
