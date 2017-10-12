// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
        private readonly string _constructorSignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgumentResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="constructorSignature">A string representing the constructor being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public ConstructorArgumentResolveOperation(Type typeBeingConstructed, string constructorSignature, string parameterName)
            : base(typeBeingConstructed, parameterName)
        {
            _constructorSignature = constructorSignature;
        }

        /// <summary>
        /// Generate the string describing what parameter was being resolved.
        /// </summary>
        /// <returns>The description string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Constants.ConstructorArgumentResolveOperation,
                ParameterName, _constructorSignature);
        }

        /// <summary>
        /// String describing the constructor being set up.
        /// </summary>
        public string ConstructorSignature => _constructorSignature;
    }
}
