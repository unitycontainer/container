using System;
using System.Reflection;

namespace Unity.Container
{
    public readonly struct ImportInfo<TInfo>
    {
        #region Fields

        public readonly TInfo      MemberInfo;
        public readonly Contract   Contract;
        public readonly bool       AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TInfo info, Type contractType, string? contractName, bool allowDefault)
        {
            MemberInfo = info;
            Contract = new Contract(contractType, contractName);
            AllowDefault = allowDefault;
        }

        public ImportInfo(TInfo info, Type contractType, bool allowDefault)
        {
            MemberInfo = info;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
        }

        #endregion
    }
}
