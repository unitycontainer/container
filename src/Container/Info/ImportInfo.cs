using System;
using System.ComponentModel.Composition;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct ImportInfo<TMemberInfo> : IImportDescriptor<TMemberInfo>
    {
        #region Fields

        private static Func<TMemberInfo, Type> DummyFunc 
            = (_) => throw new NotImplementedException("Selector is not initialized");

        private TMemberInfo _info;

        public static Func<TMemberInfo, Type> GetMemberType    = DummyFunc;
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
        public Type ContractType { get; set; }

        /// <inheritdoc />
        public string? ContractName { get; set; }

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
    }
}
