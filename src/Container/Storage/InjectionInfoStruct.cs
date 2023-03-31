using System.Diagnostics;
using Unity.Injection;

namespace Unity.Storage
{
    [DebuggerDisplay("Type: {ContractType?.Name}, Name: {ContractName}  {DataValue}")]
    public struct InjectionInfoStruct<TMember> : IInjectionInfo<TMember>
    {
        #region Fields

        public ValueData DataValue;
        public ValueData DefaultValue;

        #endregion


        #region Constructors

        public InjectionInfoStruct(TMember info, Type type)
        {
            MemberInfo = info;
            MemberType = type;
            ContractType = type;
        }

        private InjectionInfoStruct(ref InjectionInfoStruct<TMember> parent, Type type, object? data)
        {
            MemberInfo = parent.MemberInfo;
            MemberType = parent.MemberType;
            ContractType = type;

            Data = data;
        }

        #endregion


        #region Member Info

        /// <inheritdoc />
        public TMember MemberInfo { get; }

        /// <inheritdoc />
        public Type MemberType { get; }

        #endregion


        #region Metadata

        /// <inheritdoc />
        public bool IsImport { get; set; }

        /// <inheritdoc />
        public bool RequireBuild => ValueType.Unknown == DataValue.Type;

        #endregion


        #region Contract

        public Type ContractType { get; set; }

        public string? ContractName { get; set; }

        #endregion


        #region Parameters

        public object?[] Arguments
        {
            set => DataValue[ValueType.Array] = value ?? throw new ArgumentNullException(nameof(Arguments));
        }

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
                DefaultValue[ValueType.Value] = value;
            }
        }

        #endregion


        #region Value

        /// <inheritdoc />
        public object? Data
        {
            set => DataValue[ValueType.Unknown] = value;
        }

        #endregion


        #region Scope

        public InjectionInfoStruct<TMember> With(Type type, object? value)
            => new InjectionInfoStruct<TMember>(ref this, type, value);

        #endregion
    }
}
