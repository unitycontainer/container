// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;

namespace Unity.ObjectBuilder.Strategies.BuildPlan.DynamicMethod.Method
{
    /// <summary>
    /// This class records the information about which constructor argument is currently
    /// being resolved, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class MethodArgumentResolveOperation : BuildOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder.Operation.ConstructorParameterResolveOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">The type that is being constructed.</param>
        /// <param name="methodSignature">A string representing the method being called.</param>
        /// <param name="parameterName">Parameter being resolved.</param>
        public MethodArgumentResolveOperation(Type typeBeingConstructed, string methodSignature, string parameterName)
            : base(typeBeingConstructed)
        {
            MethodSignature = methodSignature;
            ParameterName = parameterName;
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

        /// <summary>
        /// Parameter that's being resolved.
        /// </summary>
        public string ParameterName { get; }
    }
}
