

using System;
using System.Globalization;
using System.Reflection;

namespace Unity.Builder.Operation
{
    /// <summary>
    /// This class records the information about which constructor argument is currently
    /// being resolved, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class MethodArgumentResolveOperation : ParameterResolveOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgumentResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="methodSignature">A string representing the method being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public MethodArgumentResolveOperation(Type typeBeingConstructed, string methodSignature, string parameterName)
            : base(typeBeingConstructed, parameterName)
        {
            MethodSignature = methodSignature;
        }

        /// <summary>
        /// Generate the string describing what parameter was being resolved.
        /// </summary>
        /// <returns>The description string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Constants.MethodArgumentResolveOperation,
                ParameterName, TypeBeingConstructed.GetTypeInfo().Name, MethodSignature);
        }

        /// <summary>
        /// String describing the method being set up.
        /// </summary>
        public string MethodSignature { get; }
    }
}
