using System;

namespace Unity
{
    public readonly struct ImportInfo<TInfo>
    {
        #region Fields

        public readonly TInfo      Info;
        public readonly object?    Data;
        public readonly Contract   Contract;
        public readonly ImportType ImportType;
        public readonly bool       AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TInfo info, Type type, string? name, object? data, bool allowDefault)
        {
            Info = info;
            AllowDefault = allowDefault;
            Contract = new Contract(type, name);
            Data = data;
            ImportType = default;
        }

        public ImportInfo(TInfo info, Type type, string? name, bool allowDefault)
        {
            Info = info;
            Contract = new Contract(type, name);
            AllowDefault = allowDefault;
            Data = default;
            ImportType = default;
        }

        #endregion
    }
}
