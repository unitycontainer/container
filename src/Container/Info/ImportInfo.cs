using System;

namespace Unity.Container
{
    public readonly struct ImportInfo<TElement>
    {
        #region Fields

        public readonly TElement Element;
        public readonly Contract Contract;
        public readonly bool     AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TElement element, Type contractType, string? contractName, bool allowDefault = false)
        {
            Element = element;
            Contract = new Contract(contractType, contractName);
            AllowDefault = allowDefault;
        }

        public ImportInfo(TElement element, Type contractType, bool allowDefault = false)
        {
            Element = element;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
        }

        #endregion
    }
}
