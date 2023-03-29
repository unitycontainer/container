using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Strategies
{
    [DebuggerDisplay("Type: {ContractType?.Name}, Name: {ContractName}  {ValueData}")]
    public struct ParameterDescriptor : IInjectionInfo<ParameterInfo>
    {
        #region Fields

        public ImportData ValueData;
        public ImportData DefaultData;

        #endregion


        #region Constructors

        public ParameterDescriptor(ParameterInfo info)
        {
            MemberInfo = info;
            
            IsImport = true;
            ContractType = info.ParameterType;

            // Default value from ParameterInfo
            if (info.HasDefaultValue) Default = info.DefaultValue;
        }

        public ParameterDescriptor(ParameterInfo info, DependencyResolutionAttribute import)
        {
            MemberInfo = info;

            ContractType = import.ContractType is not null
                ? import.ContractType
                : info.ParameterType;

            IsImport = true;
            ContractName = import.ContractName;
            AllowDefault |= import.AllowDefault;

            // Default value from ParameterInfo
            if (info.HasDefaultValue) Default = info.DefaultValue;
        }

        public ParameterDescriptor(ParameterInfo info, DefaultValueAttribute @default)
        {
            MemberInfo = info;

            IsImport = true;
            ContractType = info.ParameterType;
            Default = @default.Value;
        }

        #endregion


        #region Member Info

        /// <inheritdoc />
        public ParameterInfo MemberInfo { get; set; }

        /// <inheritdoc />
        public Type MemberType => MemberInfo.ParameterType;

        #endregion


        #region Metadata

        /// <inheritdoc />
        public bool IsImport { get; set; }

        /// <inheritdoc />
        public bool RequireBuild => ImportType.Unknown == ValueData.Type;

        #endregion


        #region Contract

        public Type ContractType { get; set; }

        public string? ContractName { get; set; }

        #endregion


        #region Parameters

        public object?[] Arguments
        {
            set => ValueData[ImportType.Array] = value ?? throw new ArgumentNullException(nameof(Arguments));
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
                DefaultData[ImportType.Value] = value;
            }
        }

        #endregion


        #region Data

        /// <inheritdoc />
        public object? Data
        {
            set => ValueData[ImportType.Unknown] = value;
        }

        #endregion


        #region Create

        public static ParameterDescriptor[] ToArray(ParameterInfo[] parameters)
        {
            ParameterDescriptor[] array = new ParameterDescriptor[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            { 
                ref var info = ref array[i];
                var parameter = parameters[i];

                info.MemberInfo = parameter;
                info.ContractType = parameter.ParameterType;
            }

            return array;
        }


        public static object[] ToArray(ParameterDescriptor[] parameters)
        {
            object[] array = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            { 
                array[i] = parameters[i].ValueData.Value!;
            }

            return array;
        }

        #endregion
    }
}
