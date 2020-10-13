using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace Unity.Container
{
    public readonly struct ImportInfo<TInfo>
    {
        #region Fields

        public readonly TInfo      Info;
        public readonly Contract   Contract;
        public readonly bool       AllowDefault;

        #endregion


        #region Constructors

        public ImportInfo(TInfo info, Type contractType, string? contractName, bool allowDefault)
        {
            Info = info;
            Contract = new Contract(contractType, contractName);
            AllowDefault = allowDefault;
        }

        public ImportInfo(TInfo info, Type contractType, bool allowDefault)
        {
            Info = info;
            Contract = new Contract(contractType);
            AllowDefault = allowDefault;
        }

        #endregion
    }


    public static class ImportInfoExtensions
    {
        public static ImportInfo<ParameterInfo> AsImportInfo(this ParameterInfo info)
        {
            ImportAttribute? attribute;

            return null == (attribute = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute)))
                ? new ImportInfo<ParameterInfo>(info, info.ParameterType, null, info.HasDefaultValue)
                : new ImportInfo<ParameterInfo>(info, attribute.ContractType ?? info.ParameterType, 
                                                      attribute.ContractName,
                                                      attribute.AllowDefault || info.HasDefaultValue);
        }
    }
}
