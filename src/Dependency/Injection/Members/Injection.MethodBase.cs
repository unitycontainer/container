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


        #region Implementation

        public override MatchRank Match(TMemberInfo other)
            => throw new System.NotImplementedException();

        public override void DescribeImport<TDescriptor>(ref TDescriptor descriptor)
            => throw new System.NotImplementedException();

        #endregion
    }
}
