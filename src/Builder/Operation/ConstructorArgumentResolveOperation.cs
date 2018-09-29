

using System;
using System.Globalization;

namespace Unity.Builder.Operation
{
    /// <summary>
    /// This class records the information about which constructor argument is currently
    /// being resolved, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class ConstructorArgumentResolveOperation : ParameterResolveOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgumentResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="constructorSignature">A string representing the constructor being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public ConstructorArgumentResolveOperation(Type typeBeingConstructed, string parameterName)
            : base(typeBeingConstructed, parameterName)
        {
        }

        /// <summary>
        /// Generate the string describing what parameter was being resolved.
        /// </summary>
        /// <returns>The description string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Constants.ConstructorArgumentResolveOperation,
                ParameterName, null);
        }
    }
}
