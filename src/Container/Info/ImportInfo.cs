using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public struct ImportInfo<TMember>
    {
        #region Fields

        public TMember Member;
        public bool    AllowDefault;
        public Type    ContractType;
        public string? ContractName;
        public ImportSource Source;
        public CreationPolicy Policy;

        #endregion
    }
}
