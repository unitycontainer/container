using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        #region Fields

        // TODO: Benchmark against virtual method
        protected static Func<TDependency, Type>? GetMemberType;
        protected static Func<TDependency, Type>? GetDeclaringType;

        #endregion


        #region Import

        public static void ProcessImport<T>(ref T info, object? value)
            where T : IInjectionInfo
        {
            do
            {
                switch (value)
                {
                    case IInjectionProvider provider:
                        provider.GetImportInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.Pipeline = iResolve.Resolve;
                        return;

                    case ResolveDelegate<PipelineContext> resolver:
                        info.Pipeline = resolver;
                        return;

                    case IResolverFactory<Type> typeFactory:
                        info.Pipeline = typeFactory.GetResolver<PipelineContext>(info.MemberType);
                        return;

                        // TODO: Alternative?
                    //case FromTypeFactory<PipelineContext> factory:
                    //    info.Pipeline = factory(info.MemberType);
                    //    return;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.AllowDefault = false;
                        return;

                    case UnityContainer.InvalidValue _:
                        return;

                    default:
                        info.Value = value;
                        return;
                }

                value = info.ImportValue;
            }
            while (ImportType.Unknown == info.ImportType);
        }

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
