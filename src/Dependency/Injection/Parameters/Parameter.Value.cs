using System;
using System.Reflection;
using Unity.Extension;

namespace Unity.Injection
{
    /// <summary>
    /// Base type for objects that are used to configure parameters for
    /// constructor or method injection, or for getting the value to
    /// be injected into a property.
    /// </summary>
    public abstract class ParameterValue : IImportProvider,
                                           IMatch<ParameterInfo, MatchRank>
    {
        #region Import Description Provider

        /// <inheritdoc/>
        public virtual void DescribeImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TContext    : IBuilderContext
            where TDescriptor : IImportDescriptor => descriptor.None();

        #endregion


        #region IMatch

        /// <summary>
        /// Checks if this parameter is compatible with the <see cref="ParameterInfo"/>
        /// </summary>
        /// <param name="other"><see cref="ParameterInfo"/> to compare to</param>
        /// <returns>True if <see cref="ParameterInfo"/> is compatible</returns>
        public virtual MatchRank Match(ParameterInfo parameter) 
            => Match(parameter.ParameterType);

        protected abstract MatchRank Match(Type type);

        #endregion
    }
}
