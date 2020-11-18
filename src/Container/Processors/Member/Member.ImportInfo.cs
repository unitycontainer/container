using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        #region Fields

        protected static Func<TDependency, Type>? GetMemberType;
        protected static Func<TDependency, Type>? GetDeclaringType;

        #endregion


        [DebuggerDisplay("ContractType: {ContractType.Name}, ContractName: {ContractName} {Data}")]
        protected struct ImportInfo : IInjectionInfo
        {
            #region Fields

            public Contract Contract;
            public ImportData Data;
            public ImportData Default;
            public ImportSource Source;
            public CreationPolicy Policy;

            #endregion


            #region Properties

            public TDependency MemberInfo  { get; set; }

            public Type ContractType
            {
                get => Contract.Type;
                set => Contract = Contract.With(value);
            }

            public string? ContractName 
            { 
                get => Contract.Name; 
                set => Contract = Contract.With(value); 
            }

            public bool AllowDefault { get; set; }

            public Type MemberType => GetMemberType!(MemberInfo);

            public Type DeclaringType => GetDeclaringType!(MemberInfo);

            public ImportType ImportType => Data.ImportType;
            
            public object? ImportValue => Data.Value;

            public Attribute[]? Attributes { get; set; }

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
                set => ProcessImport(ref this, value);
            }

            public ResolveDelegate<PipelineContext> Pipeline
            {
                set
                {
                    Data.Value = value;
                    Data.ImportType = ImportType.Pipeline;
                }
            }

            #endregion
        }
    }
}
