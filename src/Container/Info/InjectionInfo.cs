using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Container
{
    public readonly struct InjectionInfo<TInfo>
    {
        #region Fields

        public readonly ImportData        Data;
        public readonly ImportInfo<TInfo> Import;

        #endregion


        #region Constructors

        public InjectionInfo(TInfo info, Type contractType, string? contractName, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TInfo>(info, contractType, contractName, allowDefault);
            Data   = new ImportData(data, import);
        }

        public InjectionInfo(TInfo info, Type contractType, string? contractName, bool allowDefault)
        {
            Import = new ImportInfo<TInfo>(info, contractType, contractName, allowDefault);
            Data = default;
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            Data = new ImportData(data, import);
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault, object? data)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            // TODO: ??
            Data = info.AsImportData(contractType, data); 
        }

        public InjectionInfo(TInfo info, Type contractType, bool allowDefault)
        {
            Import = new ImportInfo<TInfo>(info, contractType, allowDefault);
            Data = default;
        }

        #endregion
    }
}
