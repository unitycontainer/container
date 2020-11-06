using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// </summary>
    public abstract class ParameterBase : ParameterValue,
                                          IReflectionProvider<Type>,
                                          IReflectionProvider<FieldInfo>,
                                          IReflectionProvider<PropertyInfo>
    {
        #region Fields

        protected readonly bool  AllowDefault;
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
            AllowDefault  = optional;
            ParameterType = importedType;
        }


        #endregion


        #region Reflection

        public abstract ReflectionInfo<Type> FillReflectionInfo(Type type);

        public abstract ReflectionInfo<FieldInfo> FillReflectionInfo(FieldInfo member);

        public abstract ReflectionInfo<PropertyInfo> FillReflectionInfo(PropertyInfo member);



        public ImportType FillReflectionInfo(ref ReflectionInfo<Type> reflectionInfo)
        {
            throw new NotImplementedException();
        }

        public ImportType FillReflectionInfo(ref ReflectionInfo<FieldInfo> reflectionInfo)
        {
            throw new NotImplementedException();
        }

        public ImportType FillReflectionInfo(ref ReflectionInfo<PropertyInfo> reflectionInfo)
        {
            throw new NotImplementedException();
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
