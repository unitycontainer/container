using System;

namespace Unity.Container
{
    public readonly struct ReflectionInfo<TElement>
    {
        #region Fields

        public readonly ImportInfo<TElement> Import;
        public readonly ImportData Data;

        #endregion


        #region Constructors

        public ReflectionInfo(TElement element, Type contractType, string? contractName, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType, contractName, allowDefault);
            Data   = new ImportData(data, import);
        }

        public ReflectionInfo(TElement element, Type contractType, string? contractName, bool allowDefault)
        {
            Import = new ImportInfo<TElement>(element, contractType, contractName, allowDefault);
            Data = default;
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = new ImportData(data, import);
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault, object? data)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = element.AsImportData(contractType, data); 
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = default;
        }

        public ReflectionInfo(TElement element, Type contractType, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType);
            Data = new ImportData(data, import);
        }

        #endregion
    }
}
