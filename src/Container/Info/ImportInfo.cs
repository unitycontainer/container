using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Unity.Extension;

namespace Unity.Container
{
    [DebuggerDisplay("Type: {Contract.Type?.Name}, Name: {Contract.Name}  {ValueData}")]
    public partial struct ImportInfo<TMemberInfo> : IImportDescriptor<TMemberInfo>
    {
        #region Fields

        private TMemberInfo _info;

        public static Func<TMemberInfo, Type> GetMemberType = DummyFunc;
        public static Func<TMemberInfo, Type> GetDeclaringType = DummyFunc;

        public ImportData ValueData;
        public ImportData DefaultData;

        #endregion


        #region Member Info

        /// <inheritdoc />
        public TMemberInfo MemberInfo
        {
            get => _info;
            set
            {
                _info = value;

                // TODO: Remove extra initialization from setter
                IsImport = false;
                AllowDefault = false;
                Source = ImportSource.Any;
                Policy = CreationPolicy.Any;
                ValueData.Type = ImportType.None;
                DefaultData.Type = ImportType.None;
            }
        }

        /// <inheritdoc />
        public Type MemberType => GetMemberType(_info);

        /// <inheritdoc />
        public Type DeclaringType => GetDeclaringType(_info);

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
        public Contract Contract { get; set; }

        #endregion


        #region Default Value

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
            set => ValueData[ImportType.Unknown] = value;
        }


        /// <inheritdoc />
        public Delegate Pipeline
        {
            set => ValueData[ImportType.Pipeline] = value;
        }

        #endregion


        #region Implementation

        private static Type DummyFunc(TMemberInfo _)
            => throw new NotImplementedException("Selector is not initialized");


        // TODO: Placement?
        public void FromDynamic<TContext>(ref TContext context, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportDescriptionProvider<TMemberInfo> provider:
                        ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref this);
                        break;

                    case IImportDescriptionProvider provider:
                        ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref this);
                        break;

                    case IResolve iResolve:
                        ValueData = new ImportData((ResolveDelegate<TContext>)iResolve.Resolve, ImportType.Pipeline);
                        return; 

                    case ResolveDelegate<TContext> resolver:
                        ValueData = new ImportData(resolver, ImportType.Pipeline);
                        return;

                    case IResolverFactory<Type> typeFactory:
                        ValueData = new ImportData(typeFactory.GetResolver<TContext>(MemberType), ImportType.Pipeline);
                        return;

                    case PipelineFactory<TContext> factory:
                        ValueData = new ImportData(factory(ref context), ImportType.Pipeline);
                        return;

                    case Type target when typeof(Type) != MemberType:
                        Contract = new Contract(target);
                        AllowDefault = false;
                        ValueData = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        ValueData = default;
                        return;

                    default:
                        ValueData = new ImportData(data, ImportType.Value);
                        return;
                }

                data = ValueData.Value;
            }
            while (ImportType.Unknown == ValueData.Type);
        }

        #endregion
    }
}
