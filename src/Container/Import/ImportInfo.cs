using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public struct ImportInfo<TMember> : IImportInfo
    {
        #region Fields

        public TMember   Member;
        public ImportData Data;
        private readonly Func<TMember, Type> _func;

        #endregion


        public ImportInfo(Func<TMember, Type> func)
        {
            _func = func;
            
            Data = default;
            Member = default!;
            HashCode = default;
            ContractType = default!;
            ContractName = default;
            AllowDefault = default;
            Source = ImportSource.Any;
            Policy = CreationPolicy.Any;
        }


        #region Contract

        public int     HashCode;
        public Type    ContractType { get; set; }
        public string? ContractName { get; set; }

        #endregion


        #region Properties

        public Type MemberType => _func(Member);

        public bool AllowDefault { get; set; }

        #endregion


        #region Data

        public object? ImportValue { get => Data.Value; set => Data.Value = value; }
        
        public ImportType ImportType { get => Data.ImportType; set => Data.ImportType = value; }

        #endregion


        #region Options
        
        public ImportSource Source { get; set; }
        public CreationPolicy Policy { get; set; }

        #endregion
    }
}
