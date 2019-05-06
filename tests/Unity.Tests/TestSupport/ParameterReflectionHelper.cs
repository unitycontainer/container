using System;
using System.Reflection;

namespace Unity.Tests.TestSupport
{
    /// <summary>
    /// Another reflection helper class that has extra methods
    /// for dealing with ParameterInfo.
    /// </summary>
    internal class ParameterReflectionHelper : ReflectionHelper
    {
        /// <summary>
        /// Create a new instance of <see cref="ParameterReflectionHelper"/> that
        /// lets you query information about the given ParameterInfo object.
        /// </summary>
        /// <param name="parameter">Parameter to query.</param>
        public ParameterReflectionHelper(ParameterInfo parameter) :
            base(TypeFromParameterInfo(parameter))
        {
        }

        private static Type TypeFromParameterInfo(ParameterInfo parameter)
        {
            return (parameter ?? throw new ArgumentNullException(nameof(parameter))).ParameterType;
        }
    }
}
