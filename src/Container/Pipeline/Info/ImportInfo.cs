using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public readonly struct ImportInfo<TInfo>
    {
        #region Fields

        public readonly TInfo Info;
        public readonly Contract Contract;
        public readonly bool  AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TInfo info, Type type, bool allowDefault)
        {
            Info = info;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
        }

        public ImportInfo(TInfo info, Type type, ImportAttribute? import, bool allowDefault = false)
        {
            Info = info;
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
        }

        public ImportInfo(TInfo info, Type type, object? data, bool allowDefault = false)
        {
            Info = info;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
        }

        public ImportInfo(TInfo info, Type type, string? name, bool allowDefault)
        {
            Info = info;
            Contract = new Contract(type, name);
            AllowDefault = allowDefault;
        }

        public ImportInfo(TInfo info, Type type, ImportAttribute? import, object? data, bool allowDefault = false)
        {
            Info = info;
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
        }

        #endregion
    }
}
