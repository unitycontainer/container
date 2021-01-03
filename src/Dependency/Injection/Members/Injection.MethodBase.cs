using System;
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


        #region Matching

        public override MatchRank Match(TMemberInfo other)
            => throw new System.NotImplementedException();

        #endregion

        // TODO: Selection ?
        public override void GetImportInfo<TImport>(ref TImport import)
            => throw new System.NotImplementedException();
    }
}
