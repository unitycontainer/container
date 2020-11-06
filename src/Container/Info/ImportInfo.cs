using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public struct ImportInfo<TElement>
    {
        #region Fields

        public bool    AllowDefault;
        public Type    ContractType;
        public string? ContractName;
        public readonly TElement Element;
        public readonly ImportSource Source;
        public readonly CreationPolicy Policy;

        #endregion


        #region Constructors

        public ImportInfo(TElement element, Type contractType, string? contractName, bool allowDefault = false)
        {
            Element = element;
            ContractType = contractType;
            ContractName = contractName;
            AllowDefault = allowDefault;

            Source = ImportSource.Any;
            Policy = CreationPolicy.Any;
        }

        public ImportInfo(TElement element, Type type, ImportAttribute attribute, bool allowDefault = false)
        {
            Element = element;
            ContractType = attribute.ContractType ?? type;
            ContractName = attribute.ContractName;
            Source = attribute.Source;
            AllowDefault = attribute.AllowDefault || allowDefault;
            Policy = attribute.RequiredCreationPolicy;
        }

        public ImportInfo(TElement element, Type type, ImportManyAttribute attribute, bool allowDefault = false)
        {
            Element = element;
            ContractType = attribute.ContractType ?? type;
            ContractName = attribute.ContractName;
            Source = attribute.Source;
            AllowDefault = allowDefault;
            Policy = attribute.RequiredCreationPolicy;
        }

        public ImportInfo(TElement element, Type contractType, bool allowDefault = false)
        {
            ContractType = contractType;
            ContractName = default;
            AllowDefault = allowDefault;

            Element = element;
            Source = ImportSource.Any;
            Policy = CreationPolicy.Any;
        }

        #endregion
    }
}
