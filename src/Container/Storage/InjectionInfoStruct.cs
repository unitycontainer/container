using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity.Storage
{
    [DebuggerDisplay("Member: {MemberInfo} Type: {ContractType?.Name}, Name: {ContractName ?? \"null\"} Value: {DataValue.Type}")]
    public struct InjectionInfoStruct<TMember> : IInjectionInfo<TMember>
    {
        #region Fields

        private IntPtr     _contract;
        private ContractHost _entry;

        public ValueData DataValue;
        public ValueData DefaultValue;

        #endregion


        #region Constructors

        public InjectionInfoStruct(TMember info, Type type)
        {
            MemberInfo = info;
            MemberType = type;

            _entry.Type = type;
            _entry.HashCode = Contract.GetHashCode(_entry.Type);
        }

        private InjectionInfoStruct(ref InjectionInfoStruct<TMember> parent, Type type, object? data)
        {
            MemberInfo = parent.MemberInfo;
            MemberType = parent.MemberType;

            _entry.Type = type;
            _entry.HashCode = Contract.GetHashCode(_entry.Type);

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

        #endregion


        #region Contract

        public Type ContractType
        {
            get => _entry.Type;
            set
            {
                _entry.Type = value;
                _entry.HashCode = Contract.GetHashCode(_entry.Type, _entry.Name);
            }
        }

        public string? ContractName
        {
            get => _entry.Name;
            set
            {
                _entry.Name = value;
                _entry.HashCode = Contract.GetHashCode(_entry.Type, _entry.Name);
            }
        }

        public ref Contract Contract
        {
            get
            {
                unsafe
                {
                    if (IntPtr.Zero == _contract) 
                        _contract = new IntPtr(Unsafe.AsPointer(ref _entry));

                    return ref Unsafe.AsRef<Contract>(_contract.ToPointer());
                }
            }
        }


        #endregion


        #region Parameters

        public object?[] Arguments
        {
            set => DataValue[DataType.Array] = value ?? throw new ArgumentNullException(nameof(Arguments));
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
                DefaultValue[DataType.Value] = value;
            }
        }

        #endregion


        #region Value

        /// <inheritdoc />
        public object? Data
        {
            set => DataValue[DataType.Unknown] = value;
        }

        #endregion


        #region Scope

        public InjectionInfoStruct<TMember> With(Type type, object? value)
            => new InjectionInfoStruct<TMember>(ref this, type, value);

        #endregion
    }
}
