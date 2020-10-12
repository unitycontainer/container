using System;

namespace Unity.Container
{
    public readonly struct ImportInfo<TInfo>
    {
        #region Fields

        public readonly TInfo      MemberInfo;
        public readonly Contract   Contract;
        public readonly ImportData Data;
        public readonly bool       AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TInfo info, Type contractType, string? contractName, bool allowDefault)
        {
            MemberInfo = info;
            Contract = new Contract(contractType, contractName);
            AllowDefault = allowDefault;
            Data = default;
        }

        public ImportInfo(TInfo info, Type contractType, bool allowDefault, object? data, ImportType import)
        {
            MemberInfo = info;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
            Data = new ImportData(data, import);
        }

        public ImportInfo(TInfo info, Type contractType, bool allowDefault, object? data)
        {
            MemberInfo = info;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
            Data = info.AsImportData(contractType, data);
        }

        public ImportInfo(TInfo info, Type contractType, bool allowDefault)
        {
            MemberInfo = info;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
            Data = default;
        }

        #endregion
    }
}
