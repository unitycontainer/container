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


        #region Constructors

        public ImportInfo(TMember element, Type contractType, string? contractName, bool allowDefault = false)
        {
            Member = element;
            ContractType = contractType;
            ContractName = contractName;
            AllowDefault = allowDefault;

            Source = ImportSource.Any;
            Policy = CreationPolicy.Any;
        }

        #endregion
    }
}
