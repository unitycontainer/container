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
                        info.Pipeline = (ResolveDelegate<PipelineContext>)iResolve.Resolve;
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
        protected struct ImportInfo : IInjectionInfo, IImportInfo,
                                      IImportDescriptor<TDependency>
        {
            #region Fields

            public Contract   Contract;
            public ImportData ValueData;
            public ImportData DefaultData;

            public object? this[ImportData type] { set => throw new NotImplementedException(); }

            #endregion


            #region Member Info

            /// <inheritdoc />
            public TDependency MemberInfo  { get; set; }

            /// <inheritdoc />
            public Type MemberType => GetMemberType!(MemberInfo);

            /// <inheritdoc />
            public Type DeclaringType => GetDeclaringType!(MemberInfo);

            #endregion


            #region Metadata

            /// <inheritdoc />
            public Attribute[]? Attributes { get; set; }

            /// <inheritdoc />
            public ImportSource Source { get; set; }

            /// <inheritdoc />
            public CreationPolicy Policy{ get; set; }

            #endregion


            #region Contract

            /// <inheritdoc />
            public Type ContractType
            {
                get => Contract.Type;
                set => Contract = Contract.With(value);
            }

            /// <inheritdoc />
            public string? ContractName 
            { 
                get => Contract.Name; 
                set => Contract = Contract.With(value); 
            }

            #endregion


            #region Values

            /// <inheritdoc />
            public bool AllowDefault { get; set; }

            /// <inheritdoc />
            public object? Default
            {
                set
                {
                    AllowDefault = true;
                    DefaultData[ImportType.Value] = value;
                }
            }


            #endregion


            #region Value

            /// <inheritdoc />
            public object? Value
            {
                set => ValueData[ImportType.Value] = value;
            }

            
            /// <inheritdoc />
            public object? Dynamic
            {
                set => ProcessImport(ref this, value);
            }


            /// <inheritdoc />
            public Delegate Pipeline 
            {
                set => ValueData[ImportType.Pipeline] = value;
            }

            #endregion



            #region Properties


            public ImportType ImportType => ValueData.Type;
            
            public object? ImportValue => ValueData.Value;


            #endregion
        }
    }
}
