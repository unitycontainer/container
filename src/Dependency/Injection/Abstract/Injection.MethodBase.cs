using System.Reflection;

namespace Unity.Injection
{
    public abstract class InjectionMethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]> 
                                         where TMemberInfo : MethodBase
    {
        #region Constructors

        protected InjectionMethodBase(string name, params object[] arguments)
            : base(name, arguments is not null && 0 == arguments.Length ? null : arguments)
        {
        }

        #endregion


        #region Implementation

        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (Data is null) return;

            descriptor.Arguments = Data;
        }

        #endregion
    }
}
