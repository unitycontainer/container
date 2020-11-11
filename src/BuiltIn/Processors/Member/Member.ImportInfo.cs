using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        #region Fields

        private   static Func<TDependency, Type>? _member;
        private   static Func<TDependency, Type>? _declaring;
        protected static ImportDataProvider<ImportInfo, ImportType>? ParseDataImport;

        #endregion


        [DebuggerDisplay("ContractType: {ContractType.Name}, ContractName: {ContractName} {Data}")]
        public struct ImportInfo : IInjectionInfo
        {
            #region Fields

            public int HashCode;
            public ImportData Data;
            public ImportData Default;

            #endregion


            #region Properties

            public TDependency MemberInfo  { get; set; }

            public Type ContractType { get; set; }
            public string? ContractName { get; set; }

            public bool AllowDefault { get; set; }

            public Type MemberType => _member!(MemberInfo);

            public Type DeclaringType => _declaring!(MemberInfo);

            public Contract Contract => new Contract(HashCode, ContractType, ContractName);

            #endregion


            #region Data

            public ImportType ImportType => Data.ImportType;
            
            public object? ImportValue => Data.Value;

            #endregion

            
            #region Setters

            public object? Value
            {
                set
                {
                    Data.Value = value;
                    Data.ImportType = ImportType.Value;
                }
            }

            public object? External
            {
                set => ParseDataImport!(ref this, value);
            }

            public ResolveDelegate<PipelineContext> Pipeline
            {
                set
                {
                    Data.Value = value;
                    Data.ImportType = ImportType.Pipeline;
                }
            }

            public void UpdateHashCode() => HashCode = Contract.GetHashCode(ContractType, ContractName);

            #endregion


            #region Options

            public ImportSource Source { get; set; }
            public CreationPolicy Policy { get; set; }

            #endregion
        }
    }
}
