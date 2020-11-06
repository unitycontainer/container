using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// </summary>
    public abstract class ParameterBase : ParameterValue,
                                          IReflectionProvider<FieldInfo>,
                                          IReflectionProvider<PropertyInfo>
    {
        #region Fields

        protected readonly bool AllowDefault;
        protected readonly Type? ParameterType;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ParameterBase"/> that holds information
        /// about type of import the parameter is injected with
        /// </summary>
        /// <param name="importedType"><see cref="Type"/> to inject</param>
        protected ParameterBase(Type? importedType, bool optional)
        {
            AllowDefault = optional;
            ParameterType = importedType;
        }


        #endregion


        #region Reflection

        public virtual ImportType FillReflectionInfo(ref ReflectionInfo<FieldInfo> reflectionInfo)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                reflectionInfo.Import.ContractType = ParameterType;

            reflectionInfo.Import.AllowDefault |= AllowDefault;

            return reflectionInfo.Data.DataType;
        }

        public virtual ImportType FillReflectionInfo(ref ReflectionInfo<PropertyInfo> reflectionInfo)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                reflectionInfo.Import.ContractType = ParameterType;

            reflectionInfo.Import.AllowDefault |= AllowDefault;

            return reflectionInfo.Data.DataType;
        }


        public override ImportType FillReflectionInfo(ref ReflectionInfo<ParameterInfo> reflectionInfo)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                reflectionInfo.Import.ContractType = ParameterType;

            reflectionInfo.Import.AllowDefault |= AllowDefault || reflectionInfo.Import.Element.HasDefaultValue;

            return reflectionInfo.Data.DataType;
        }


        #endregion


        #region Implementation

        public override MatchRank Match(Type type)
        {
            // TODO: Cases in between
            return null == ParameterType 
                ? MatchRank.ExactMatch
                : ParameterType.MatchTo(type);
        }

        public override MatchRank Match(ParameterInfo parameter) => 
            Match(parameter.ParameterType);

        #endregion
    }
}
