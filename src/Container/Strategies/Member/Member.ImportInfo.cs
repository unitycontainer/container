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

        public static void ProcessImport(ref ImportInfo info, object? value)
        {
            do
            {
                switch (value)
                {
                    case IImportDescriptionProvider provider:
                        provider.DescribeImport(ref info);
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


        [DebuggerDisplay("ContractType: {ContractType?.Name}, ContractName: {ContractName}  {ValueData}")]
        public struct ImportInfo : IImportInfo,
                                   IInjectionInfo, 
                                   IImportDescriptor<TDependency>
        {
            #region Fields

            private TDependency _info;
            public Contract Contract; // TODO: Requires optimization
            public ImportData ValueData;
            public ImportData DefaultData;

            #endregion


            #region Member Info

            /// <inheritdoc />
            public TDependency MemberInfo
            {
                get => _info;
                set
                {
                    _info = value;
                    IsImport = false;
                    AllowDefault = false;
                    Source = ImportSource.Any;
                    Policy = CreationPolicy.Any;
                    ValueData.Type = ImportType.None;
                    DefaultData.Type = ImportType.None;
                }
            }

            /// <inheritdoc />
            public Type MemberType => GetMemberType!(MemberInfo);

            /// <inheritdoc />
            public Type DeclaringType => GetDeclaringType!(MemberInfo);

            #endregion


            #region Metadata

            /// <inheritdoc />
            public bool IsImport { get; set; }

            /// <inheritdoc />
            public Attribute[]? Attributes { get; set; }

            /// <inheritdoc />
            public ImportSource Source { get; set; }

            /// <inheritdoc />
            public CreationPolicy Policy { get; set; }

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
