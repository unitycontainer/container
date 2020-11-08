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

        public virtual ImportData GetReflectionInfo(ref ImportInfo<FieldInfo> info)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                info.ContractType = ParameterType;

            info.AllowDefault |= AllowDefault;

            return default;
        }

        public virtual ImportData GetReflectionInfo(ref ImportInfo<PropertyInfo> info)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                info.ContractType = ParameterType;

            info.AllowDefault |= AllowDefault;

            return default;
        }

        public override ImportData GetReflectionInfo(ref ImportInfo<ParameterInfo> info)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                info.ContractType = ParameterType;

            info.AllowDefault |= AllowDefault || info.Member.HasDefaultValue;

            return default;
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

        public override MatchRank Match(ParameterInfo parameter) 
            => Match(parameter.ParameterType);

        #endregion
    }
}
