using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// Base type for objects that are used to configure parameters for
    /// constructor or method injection, or for getting the value to
    /// be injected into a property.
    /// </summary>
    public abstract class ParameterValue : IMatch<Type>,
                                           IMatch<ParameterInfo>,
                                           IReflectionProvider<Type>,
                                           IReflectionProvider<FieldInfo>,
                                           IReflectionProvider<PropertyInfo>,
                                           IReflectionProvider<ParameterInfo>
    {
        #region IMatch

        /// <summary>
        /// Checks if this parameter is compatible with the <see cref="ParameterInfo"/>
        /// </summary>
        /// <param name="other"><see cref="ParameterInfo"/> to compare to</param>
        /// <returns>True if <see cref="ParameterInfo"/> is compatible</returns>
        public abstract MatchRank Match(ParameterInfo parameter);

        /// <summary>
        /// Checks if this parameter is compatible with the type
        /// </summary>
        /// <param name="type"><see cref="Type"/> to compare to</param>
        /// <returns>True if <see cref="Type"/> is equal</returns>
        public abstract MatchRank Match(Type type);

        #endregion


        #region IReflectionProvider

        public virtual ImportData GetReflectionInfo(ref ImportInfo<Type> info) 
            => GetReflectionInfo(ref info, info.Member);

        public virtual ImportData GetReflectionInfo(ref ImportInfo<ParameterInfo> info)
            => GetReflectionInfo(ref info, info.Member.ParameterType);

        public ImportData GetReflectionInfo(ref ImportInfo<FieldInfo> info)
            => GetReflectionInfo(ref info, info.Member.FieldType);

        public ImportData GetReflectionInfo(ref ImportInfo<PropertyInfo> info)
            => GetReflectionInfo(ref info, info.Member.PropertyType);

        #endregion


        #region Implementation

        protected virtual ImportData GetReflectionInfo<T>(ref ImportInfo<T> info, Type type) 
            => default;

        #endregion
    }
}
